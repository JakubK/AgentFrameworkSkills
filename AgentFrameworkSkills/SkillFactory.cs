using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using AgentFrameworkSkills.Exceptions;
using AgentFrameworkSkills.Models;
using AgentFrameworkSkills.Validators;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AgentFrameworkSkills;

public static class SkillFactory
{
    /// <summary>
    /// Regex used for reading Skill file, allowing to extract the YAML metadata part and Markdown content part
    /// </summary>
    private static readonly Regex FrontMatterRegex = new(@"^---\s*(.*?)\s*---\s*(.*)$", RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly HttpClient GithubHttpClient = new();
    
    /// <summary>
    /// Loads and validates Skill from given path to Skill file
    /// </summary>
    /// <param name="pathToFile">Path to Skill file, name can be anything</param>
    /// <returns></returns>
    /// <exception cref="SkillFileException"></exception>
    public static async Task<Skill> FromFileAsync(string pathToFile)
    {
        var content = await File.ReadAllTextAsync(pathToFile);
        var match = FrontMatterRegex.Match(content);

        if (!match.Success)
            throw new SkillFileException("Missing or invalid YAML front matter.");

        var yaml = match.Groups[1].Value;
        var body = match.Groups[2].Value;
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        
        var front = deserializer.Deserialize<SkillFront>(yaml);
        var frontValidator = new SkillFrontValidator();
        var validationResults = await frontValidator.ValidateAsync(front);
        
        if(!validationResults.IsValid)
        {
            var sb = new StringBuilder();
            
            foreach(var failure in validationResults.Errors)
            {
                sb.AppendLine($"Property {failure.PropertyName} failed validation. Error was: {failure.ErrorMessage}");
            }
            
            throw new SkillFileException(sb.ToString());
        }

        var directory = Path.GetFileName(Path.GetDirectoryName(pathToFile));
        if (directory != front.Name)
        {
            throw new SkillFileException("Skill name should be equal to its parent directory name");
        }

        return new Skill
        (
            front,
            body.Trim(),
            directory
        );
    }

    /// <summary>
    /// Loads and validates multiple Skills from given path directory containing subdirectories of skills
    /// Expects that in each subdirectory there is a file named "SKILL.md"
    /// </summary>
    /// <param name="pathToDirectory">Path to root directory of skills</param>
    /// <returns></returns>
    /// <exception cref="SkillFileException"></exception>
    public static async Task<IEnumerable<Skill>> FromSkillsRootAsync(string pathToDirectory)
    {
        var skills = new List<Skill>();
        var subdirectories = Directory.GetDirectories(pathToDirectory);
        foreach (var subdirectory in subdirectories)
        {
            var skill = await FromFileAsync(Path.Combine(subdirectory, "SKILL.md"));
            skills.Add(skill);
        }
        
        return skills;
    }

    /// <summary>
    /// Loads and validates skill from provided GitHub repository
    /// </summary>
    /// <param name="ownerAndRepo">GitHub Owner and repo separated by '/' </param>
    /// <param name="pathToSkillDir">Path to directory containing SKILL.md</param>
    /// <param name="localSkillsRoot">Root path of where downloaded skills will be placed</param>
    /// <param name="token">Optional Github token for private skills</param>
    /// <returns></returns>
    public static async Task<Skill> FromGitHub(string ownerAndRepo, string pathToSkillDir, string localSkillsRoot, string? token)
    {
        await DownloadGithubDirectoryAsync(ownerAndRepo, pathToSkillDir, localSkillsRoot, token);

        var directoryName = pathToSkillDir.Split("/").Last();
        return await FromFileAsync(Path.Combine(localSkillsRoot, directoryName));
    }
    
    private static async Task DownloadGithubDirectoryAsync(
        string ownerAndRepo,
        string directoryPath,
        string targetRoot,
        string? token = null)
    {
        // Required User-Agent header for GitHub
        GithubHttpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("MyApp", "1.0"));

        if (!string.IsNullOrEmpty(token))
        {
            GithubHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", token);
        }
        
        var skillName = directoryPath.Split("/").Last();
        Directory.CreateDirectory(Path.Combine(targetRoot, skillName));

        async Task DownloadDir(string path, string localRoot)
        {
            var apiUrl =
                $"https://api.github.com/repos/{ownerAndRepo}/contents/{path}";

            var json = await GithubHttpClient.GetStringAsync(apiUrl);
            var items = JsonDocument.Parse(json).RootElement;
            
            Directory.CreateDirectory(localRoot);

            foreach (var item in items.EnumerateArray())
            {
                var type = item.GetProperty("type").GetString();
                var name = item.GetProperty("name").GetString()!;
                var itemPath = string.IsNullOrEmpty(path) ? name : $"{path}/{name}";
                var localPath = Path.Combine(localRoot, skillName, name);

                if (type == "dir")
                {
                    await DownloadDir(itemPath, localPath);
                }
                else if (type == "file")
                {
                    var downloadUrl = item.GetProperty("download_url").GetString()!;
                    var bytes = await GithubHttpClient.GetByteArrayAsync(downloadUrl);
                    await File.WriteAllBytesAsync(localPath, bytes);
                }
            }
        }

        await DownloadDir(directoryPath, targetRoot);
    }
}