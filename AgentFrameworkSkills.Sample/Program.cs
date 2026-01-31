using System.ComponentModel;
using AgentFrameworkSkills.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = "http://localhost:11434";
var modelName = "qwen3:0.6b";

var skill = new Skill("someName", "someDesc", "someContent");

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";

// Get a chat client for Ollama and use it to construct an AIAgent.
AIAgent agent = new OllamaApiClient(new Uri(endpoint), modelName)
    .AsAIAgent(instructions: "You are a helpful assistant.", name: "Test", tools: [AIFunctionFactory.Create(GetWeather)]);

// Invoke the agent and output the text result.
var response = await agent.RunAsync("Hello");
Console.WriteLine(response);