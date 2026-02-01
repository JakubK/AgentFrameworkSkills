using AgentFrameworkSkills.Exceptions;

namespace AgentFrameworkSkills.Tests;

public class SkillReaderTests
{
    [Test]
    public async Task FromFileAsync_WhenValidSkill_LoadsAllData()
    {
        var skill = await SkillReader.FromFileAsync("TestData/some-skill/SKILL.md");

        await Assert.That(skill.SkillFront.Name).IsEqualTo("some-skill");
        await Assert.That(skill.SkillFront.Description).IsEqualTo("A description of what this skill does and when to use it.");
    }
    
    [Test]
    public async Task FromFileAsync_WhenSkillNameDifferentThanParentDir_Throws()
    {
        await Assert.ThrowsAsync<SkillFileException>(async () =>
        {
            await SkillReader.FromFileAsync("TestData/some-skill/SKILL2.md");
        });
    }
}
