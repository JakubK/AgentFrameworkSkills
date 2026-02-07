using System.ComponentModel;
using AgentFrameworkSkills.Models;

namespace AgentFrameworkSkills;

public class SkillTools(IEnumerable<Skill> allSkills)
{
    [Description("""
     Load the full content of a skill into the agent's context.
     
     Use this when you need detailed information about how to handle a specific
     type of request. This will provide you with comprehensive instructions,
     policies, and guidelines for the skill area.
    """)]
    public string LoadSkill([Description("skillName: The name of the skill to load")] string skillName)
    {
        var skill = allSkills.FirstOrDefault(x => x.SkillFront.Name == skillName);
        if (skill != null)
        {
            return $"""
                Loaded skill {skillName}:  
                {skill.Content}
                """;
        }
        
        var availableSkills = string.Join(", ", allSkills.Select(x => x.SkillFront.Name));
        return $"""
            Skill {skillName} was not found.
            Available skills:
            {availableSkills}
            """;
    }

    [Description("""
                  Downloads skill from GitHub and stores it in given location.
                  The resulting skill content is then placed in the context.
                  Before downloading, the provided localSkillsRoot is checked
                 """)]
    public async Task<string> LoadOrDownloadSkill([Description("ownerAndRepo: owner of repository and name of repository separated by slash")] string ownerAndRepo,
        [Description("pathToSkillDir: Path in GitHub repo root to directory containing SKILL.md file") ] string pathToSkillDir,
        [Description("localSkillsRoot: Path to root directory on this machine, containing directories of skills")] string localSkillsRoot,
        [Description("token: Optional GitHub Token used when Skill is located in Private repository")] string? token)
    {
        try
        {
            var skillName = pathToSkillDir.Split("/").Last();
            Skill? skill;
            var localSkillPath = Path.Combine(localSkillsRoot, skillName);
            if (Path.Exists(localSkillPath))
            {
                skill = await SkillReader.FromFileAsync(localSkillPath);
            }
            else
            {
                skill = await SkillDownloader.FromGitHub(ownerAndRepo, pathToSkillDir, localSkillsRoot, token);
            }
            return $"""
                    Loaded skill {skill.SkillFront.Name}:  
                    {skill.Content}
                    """;
        }
        catch (Exception ex)
        {
            return $"""
                    There was an error when trying to load skill {ownerAndRepo}/{pathToSkillDir}
                    {ex.Message}
                    """;
        }
    }
}