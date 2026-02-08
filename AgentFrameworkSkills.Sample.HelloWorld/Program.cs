using AgentFrameworkSkills;
using AgentFrameworkSkills.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = "http://localhost:11434";
var modelName = "qwen3:4b";

var skills = (await SkillFactory.FromSkillsRootAsync("Skills")).ToList();

var skillTools = new SkillTools(skills);

var agent = new OllamaApiClient(new Uri(endpoint), modelName)
    .AsAIAgent(instructions: "You are a helpful assistant", name: "Test", tools: [AIFunctionFactory.Create(skillTools.LoadSkill)])
    .AsBuilder()
    .UseSkills(skills)
    .Build();

var session = await agent.CreateSessionAsync();
Console.WriteLine(await agent.RunAsync("Greet Mark", session));
Console.WriteLine("----");
Console.WriteLine(await agent.RunAsync("Greet Tom", session));
