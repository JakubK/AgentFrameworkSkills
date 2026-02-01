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
}