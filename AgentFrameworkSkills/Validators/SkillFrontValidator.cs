using System.Text.RegularExpressions;
using AgentFrameworkSkills.Models;
using FluentValidation;

namespace AgentFrameworkSkills.Validators;

public class SkillFrontValidator : AbstractValidator<SkillFront>
{
    /// <summary>
    /// Only Unicode lowercase letters, digits, and hyphens (a-z, 0-9, -).
    /// Cannot start or end with a hyphen.
    /// Cannot contain consecutive hyphens.
    /// </summary>
    private static readonly Regex NameRegex = new (@"^(?!.*--)[\p{Ll}0-9]+(?:-[\p{Ll}0-9]+)*$");

    /// <summary>
    /// Space-separated list of values Regex
    /// </summary>
    private static readonly Regex AllowedToolsRegex = new (@"^(\S+)(\s\S+)*$");
    
    public SkillFrontValidator()
    {
        RuleFor(s => s.Name)
            .MinimumLength(1)
            .MaximumLength(64);
            
        RuleFor(s => s.Name)    
            .Matches(NameRegex)
            .WithMessage("Only Unicode lowercase letters, digits, and hyphens, not starting with hyphen and not containing consecutive hyphens are allowed.");

        RuleFor(s => s.Description)
            .MinimumLength(1)
            .MaximumLength(1024);

        RuleFor(s => s.License)
            .MinimumLength(1)
            .MaximumLength(500)
            .When(s => !string.IsNullOrEmpty(s.License));

        RuleFor(s => s.AllowedTools)
            .Matches(AllowedToolsRegex)
            .WithMessage("Value should be a string with values space separated.")
            .When(s => !string.IsNullOrEmpty(s.AllowedTools));
    }
}