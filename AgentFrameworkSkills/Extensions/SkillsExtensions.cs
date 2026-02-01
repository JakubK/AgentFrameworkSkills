using AgentFrameworkSkills.Models;
using Microsoft.Agents.AI;

namespace AgentFrameworkSkills.Extensions;

public static class SkillsExtensions
{
    /// <summary>
    /// Extension which registers SkillMiddleware
    /// </summary>
    /// <param name="aiAgentBuilder"></param>
    /// <param name="skills"></param>
    /// <returns></returns>
    public static AIAgentBuilder UseSkills(this AIAgentBuilder aiAgentBuilder, IEnumerable<Skill> skills)
    {
        return aiAgentBuilder.Use(SkillMiddleware.Apply(skills), null);
    }
}