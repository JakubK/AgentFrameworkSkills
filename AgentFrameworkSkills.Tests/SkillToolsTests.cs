namespace AgentFrameworkSkills.Tests;

public class SkillToolsTests
{
    [Test]
    public async Task LoadSkill_WhenSkillMissing_DoesNotLoadIt()
    {
        var sut = new SkillTools([]);
        var skillName = "missing-skill";
        var result = sut.LoadSkill(skillName);
        var phrase = $"Skill {skillName} was not found.";

        await Assert.That(result).Contains(phrase);
    }
    
    [Test]
    public async Task LoadSkill_WhenSkillAvailable_LoadsIt()
    {
        var sut = new SkillTools([]);
        var skillName = "missing-skill";
        var result = sut.LoadSkill(skillName);
        var phrase = $"Skill {skillName} was not found.";

        await Assert.That(result).Contains(phrase);
    }
}