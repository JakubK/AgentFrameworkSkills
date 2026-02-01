namespace AgentFrameworkSkills.Models;

public class SkillFront
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    
    public string? License { get; set; }
    
    public Dictionary<string, string>? Metadata { get; set; }
    
    public string? Compatibility { get; set; }
    public string? AllowedTools { get; set; }
}
