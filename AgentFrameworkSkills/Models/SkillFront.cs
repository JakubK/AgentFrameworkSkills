namespace AgentFrameworkSkills.Models;

public class SkillFront
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public string? License { get; set; }
    
    public Dictionary<string, string>? Metadata { get; set; }
    
    public string? Compatiblity { get; set; }
    public string? AllowedTools { get; set; }
}
