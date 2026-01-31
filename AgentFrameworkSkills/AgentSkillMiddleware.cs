using System.Text;
using AgentFrameworkSkills.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkSkills;

public static class AgentSkillMiddleware
{
    public static Func<IEnumerable<ChatMessage>,
        AgentSession?,
        AgentRunOptions?,
        AIAgent,
        CancellationToken,
        Task<AgentResponse>> InformAboutSkillsFactory(IEnumerable<Skill> skills)
    {
        return async (messages, session, options, innerAgent, cancellationToken) =>
        {
            var messageList = messages.ToList();

            var sb = new StringBuilder();
            sb.AppendLine("## Available Skills:");
            foreach (var skill in skills)
            {
                sb.AppendLine($"- **{skill.Name}**: {skill.Description}");
            }

            sb.AppendLine("Use the loadSkill tool when you need detailed information");

            // Find the existing system message (if any)
            var sysIndex = messageList.FindIndex(m => m.Role == ChatRole.System);

            if (sysIndex >= 0)
            {
                var existing = messageList[sysIndex].Text;
                var updated = existing + "\n\n" + sb;
                messageList[sysIndex] = new ChatMessage(ChatRole.System, updated);
            }
            else
            {
                messageList.Insert(0, new ChatMessage(ChatRole.System, sb.ToString()));
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