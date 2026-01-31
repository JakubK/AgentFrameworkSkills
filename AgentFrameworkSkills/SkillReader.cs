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
    /// Regex used for reading SKILL.md, allowing to extract the YAML metadata part and Markdown content part
    /// </summary>
    private static readonly Regex FrontMatterRegex = new(@"^---\s*(.*?)\s*---\s*(.*)$", RegexOptions.Singleline | RegexOptions.Compiled);
    
    public static async Task<Skill> FromFileAsync(string pathToFile)
    {
        var content = await File.ReadAllTextAsync(pathToFile);
        
        var match = FrontMatterRegex.Match(content);

        if (!match.Success)
            throw new InvalidOperationException("Missing or invalid YAML front matter.");

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

        if (Path.GetDirectoryName(pathToFile) != front.Name)
        {
            throw new SkillFileException("Skill name should be equal to its parent directory name");
        }

        return new Skill
        (
            front.Name,
            front.Description,
            front.License,
            front.Metadata,
            front.Compatibility,
            front.AllowedTools,
            body
        );
    }
}