using System.Text;
using System.Text.RegularExpressions;
using AgentFrameworkSkills.Exceptions;
using AgentFrameworkSkills.Models;
using AgentFrameworkSkills.Validators;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AgentFrameworkSkills;

public static class SkillReader
{
    /// <summary>
    /// Regex used for reading Skill file, allowing to extract the YAML metadata part and Markdown content part
    /// </summary>
    private static readonly Regex FrontMatterRegex = new(@"^---\s*(.*?)\s*---\s*(.*)$", RegexOptions.Singleline | RegexOptions.Compiled);
    
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

        if (Path.GetFileName(Path.GetDirectoryName(pathToFile)) != front.Name)
        {
            throw new SkillFileException("Skill name should be equal to its parent directory name");
        }

        return new Skill
        (
            front,
            body.Trim()
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
        var skills = new  List<Skill>();
        var subdirectories = Directory.GetDirectories(pathToDirectory);
        foreach (var subdirectory in subdirectories)
        {
            var skill = await FromFileAsync(Path.Combine(subdirectory, "SKILL.md"));
            skills.Add(skill);
        }
        
        return skills;
    }
}
