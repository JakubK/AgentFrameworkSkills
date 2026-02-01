using AgentFrameworkSkills.Models;
using AgentFrameworkSkills.Validators;

namespace AgentFrameworkSkills.Tests;

public class SkillFrontValidatorTests
{
    [Test]
    [Arguments("pdf-processing")]
    [Arguments("excel")]
    public async Task WhenNameValid_DoesNotThrow(string name)
    {
        var sut = new SkillFrontValidator();
        var skill = new SkillFront
        {
            Name = name,
            Description = "Some description"
        };
        
        var result = await sut.ValidateAsync(skill);
        await Assert.That(result.IsValid).IsTrue();
    }
    
    [Test]
    [Arguments("Pdf-processing")]
    [Arguments("pdf--processing")]
    [Arguments("-pdf")]
    public async Task WhenNameInvalid_Throws(string name)
    {
        var sut = new SkillFrontValidator();
        var skill = new SkillFront
        {
            Name = name,
            Description = "Some description"
        };
        
        var result = await sut.ValidateAsync(skill);
        await Assert.That(result.IsValid).IsFalse();
    }
}