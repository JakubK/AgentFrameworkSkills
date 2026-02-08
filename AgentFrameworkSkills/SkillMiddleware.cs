using System.Text;
using AgentFrameworkSkills.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkSkills;

public static class SkillMiddleware
{
    /// <summary>
    /// Func generating middleware which will paste information about provided skills
    /// In the System Prompt
    /// </summary>
    /// <param name="skills">List of skills</param>
    /// <param name="customSkillPrompt">Own system prompt append text with info about skills</param>
    /// <returns></returns>
    public static Func<IEnumerable<ChatMessage>,
        AgentSession?,
        AgentRunOptions?,
        AIAgent,
        CancellationToken,
        Task<AgentResponse>> Apply(IEnumerable<Skill> skills, Func<IEnumerable<Skill>, string>? customSkillPrompt = null)
    {
        return async (messages, session, options, innerAgent, cancellationToken) =>
        {
            var messageList = messages.ToList();

            string skillPrompt;
            if (customSkillPrompt != null)
            {
                skillPrompt = customSkillPrompt(skills);
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("## Available Skills:");
                foreach (var skill in skills)
                {
                    sb.AppendLine($"- **{skill.SkillFront.Name}**: {skill.SkillFront.Description} {skill.Directory}");
                }

                sb.AppendLine("Use the loadSkill tool when you need detailed information");
                
                skillPrompt = sb.ToString();
            }

            // Drop the skills prompt only when it was not found
            var skillPromptUsed = messageList.Any(m => m.Text == skillPrompt);
            if (!skillPromptUsed)
            {
                messageList.Insert(0, new ChatMessage(ChatRole.User, skillPrompt));
            }

            return await innerAgent.RunAsync(
                messageList,
                session,
                options,
                cancellationToken
            );
        };
    }
}