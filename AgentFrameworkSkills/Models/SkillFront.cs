namespace AgentFrameworkSkills.Models;

public record SkillFront(
    string Name,
    string Description,
    string? License = null,
    Dictionary<string, string>? Metadata = null,
    string? Compatibility = null,
    string? AllowedTools = null
);