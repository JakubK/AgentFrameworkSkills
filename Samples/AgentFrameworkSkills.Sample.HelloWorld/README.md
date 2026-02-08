### HelloWorld example

This sample uses qwen3:4b with OllamaSharp to demonstrate simplest use of skills.
Skill which is greeting users is loaded from disk on startup.
All skills are held in-memory, but in context there is only name and description until tool for specific skill loading is used.

Middleware is being registered for informing Agent about skills which can be loaded.