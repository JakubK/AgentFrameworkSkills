namespace AgentFrameworkSkills.Models;

public record SkillFront(
    string Name,
    string Description,
    string? License,
    Dictionary<string, string>? Metadata,
    string? Compatibility,
    string? AllowedTools
);