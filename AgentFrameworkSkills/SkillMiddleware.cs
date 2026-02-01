using System.Text;
using AgentFrameworkSkills.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkSkills;

public static class SkillMiddleware
{
    public static Func<IEnumerable<ChatMessage>,
        AgentSession?,
        AgentRunOptions?,
        AIAgent,
        CancellationToken,
        Task<AgentResponse>> InformAboutSkillsFactory(IEnumerable<Skill> skills, Func<IEnumerable<Skill>, string>? customSkillPrompt = null)
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
                    sb.AppendLine($"- **{skill.SkillFront.Name}**: {skill.SkillFront.Description}");
                }

                sb.AppendLine("Use the loadSkill tool when you need detailed information");
                
                skillPrompt = sb.ToString();
            }

            // Find the existing system message (if any)
            var sysIndex = messageList.FindIndex(m => m.Role == ChatRole.System);

            if (sysIndex >= 0)
            {
                var existing = messageList[sysIndex].Text;
                var updated = existing + "\n\n" + skillPrompt;
                messageList[sysIndex] = new ChatMessage(ChatRole.System, updated);
            }
            else
            {
                messageList.Insert(0, new ChatMessage(ChatRole.System, skillPrompt));
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