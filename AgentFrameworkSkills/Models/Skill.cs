namespace AgentFrameworkSkills.Models;

public record Skill(
    string Name,
    string Description,
    string Content,
    string? License = null,
    Dictionary<string, string>? Metadata = null,
    string? Compatibility = null,
    string? AllowedTools = null
);
