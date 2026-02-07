using AgentFrameworkSkills;
using AgentFrameworkSkills.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = "http://localhost:11434";
var modelName = "qwen3:0.6b";

var skills = (await SkillReader.FromSkillsRootAsync("Skills")).ToList();

var skillTools = new SkillTools(skills);

var agent = new OllamaApiClient(new Uri(endpoint), modelName)
    .AsAIAgent(instructions: "You are a helpful assistant.", name: "Test", tools: [AIFunctionFactory.Create(skillTools.LoadSkill)])
    .AsBuilder()
    .UseSkills(skills)
    .Build();

// Invoke the agent and output the text result.
var response = await agent.RunAsync("Hello");
Console.WriteLine(response);