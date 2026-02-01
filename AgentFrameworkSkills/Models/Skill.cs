namespace AgentFrameworkSkills.Models;

public record Skill(
    SkillFront SkillFront,
    string Content
)
{
    public Skill(string name, string description, string content) : this(new SkillFront
    {
        Name = name,
        Description = description,
    }, content)
    {
    }
}
