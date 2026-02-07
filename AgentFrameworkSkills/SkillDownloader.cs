using System.Net.Http.Headers;
using System.Text.Json;
using AgentFrameworkSkills.Models;

namespace AgentFrameworkSkills;

/// <summary>
/// Service for downloading skills from GitHub
/// </summary>
public class SkillDownloader
{
    private static readonly HttpClient _githubHttpClient = new ()
    {
        BaseAddress = new Uri(""),
    };

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
        return await SkillReader.FromFileAsync(Path.Combine(localSkillsRoot, directoryName));
    }
    
    private static async Task DownloadGithubDirectoryAsync(
        string ownerAndRepo,
        string directoryPath,
        string targetRoot,
        string? token = null)
    {
        // Required User-Agent header for GitHub
        _githubHttpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("MyApp", "1.0"));

        if (!string.IsNullOrEmpty(token))
        {
            _githubHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("token", token);
        }

        async Task DownloadDir(string path, string localRoot)
        {
            var apiUrl =
                $"https://api.github.com/repos/{ownerAndRepo}/contents/{path}";

            var json = await _githubHttpClient.GetStringAsync(apiUrl);
            var items = JsonDocument.Parse(json).RootElement;

            Directory.CreateDirectory(localRoot);

            foreach (var item in items.EnumerateArray())
            {
                var type = item.GetProperty("type").GetString();
                var name = item.GetProperty("name").GetString()!;
                var itemPath = string.IsNullOrEmpty(path) ? name : $"{path}/{name}";
                var localPath = Path.Combine(localRoot, name);

                if (type == "dir")
                {
                    await DownloadDir(itemPath, localPath);
                }
                else if (type == "file")
                {
                    var downloadUrl = item.GetProperty("download_url").GetString()!;
                    var bytes = await _githubHttpClient.GetByteArrayAsync(downloadUrl);
                    await File.WriteAllBytesAsync(localPath, bytes);
                }
            }
        }

        await DownloadDir(directoryPath, targetRoot);
    }
}