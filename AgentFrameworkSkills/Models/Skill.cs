namespace AgentFrameworkSkills.Models;

public record Skill(
    string Name,
    string Description,
    string? License,
    Dictionary<string, string>? Metadata,
    string? Compatibility,
    string? AllowedTools,
    string Content
) : SkillFront(Name, Description, License, Metadata, Compatibility, AllowedTools);
