using AgentFrameworkSkills;
using AgentFrameworkSkills.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = "http://localhost:11434";
var modelName = "qwen3:0.6b";

var skill = new Skill("someName", "someDesc", "someContent");

var allSkills = new List<Skill>
{
    skill
};

var skillTools = new SkillTools(allSkills);
var agent = new OllamaApiClient(new Uri(endpoint), modelName)
    .AsAIAgent(instructions: "You are a helpful assistant.", name: "Test", tools: [AIFunctionFactory.Create(skillTools.LoadSkill)])
    .AsBuilder()
    .Use(AgentSkillMiddleware.InformAboutSkillsFactory(allSkills), null)
    .Build();

// Invoke the agent and output the text result.
var response = await agent.RunAsync("Hello");
Console.WriteLine(response);