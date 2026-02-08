using System.ComponentModel;
using AgentFrameworkSkills.Models;

namespace AgentFrameworkSkills;

public class SkillTools(List<Skill> allSkills)
{
    [Description("""
     Load the full content of a skill into the agent's context.
     
     Use this always when you a skill looks suitable to handle user request. This will provide you with comprehensive instructions,
     policies, and guidelines for the skill area.
     Never assume skill is not needed
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
        [Description("token: Optional GitHub Token used when Skill is located in Private repository")] string? token = null)
    {
        try
        {
            var skillName = pathToSkillDir.Split("/").Last();
            var localSkillPath = Path.Combine(localSkillsRoot, skillName, "SKILL.md");

            Skill skill;
            if (allSkills.Any(x => x.SkillFront.Name == skillName))
            {
                skill = allSkills.First(x => x.SkillFront.Name == skillName);
            }
            else
            {
                skill = Path.Exists(localSkillPath)
                    ? await SkillFactory.FromFileAsync(localSkillPath)
                    : await SkillFactory.FromGitHub(ownerAndRepo, pathToSkillDir, localSkillsRoot, token);
                
                allSkills.Add(skill);
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