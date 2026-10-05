using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("No se encontró la variable de entorno OPENAI_API_KEY.");
    return;
}

var model = "gpt-5-nano"; // or "gpt-4o", "gpt-4o-mini", etc.
var client = new OpenAIClient(apiKey);
var chatClient = client.GetChatClient(model);
var prompt = "Qué es el NexaSense XR4?";

// var aiAgent = chatClient.AsAIAgent(name: "Mi primer Agente",
//     instructions: "Eres un agente muy útil");

ChatClientAgentOptions options = new()
{
    // Esto es antiguo y por eso se comenta
    // ChatOptions = new()
    // {
    //     MaxTokens = 1000,
    //     RawRepresentationFactory = _ => new ChatCompletionOptions()
    //     {
    //         ReasoningEffortLevel = ChatReasoningEffortLevel.Minimal,
    //     }
    // }
    ChatOptions = new()
    {
        MaxOutputTokens = 1000,
        Reasoning = new ReasoningOptions { Effort = ReasoningEffort.Low }
    }
};

// ChatClientAgent aiAgent = new(chatClient.AsIChatClient(), options);
// var response = await aiAgent.RunAsync(prompt);
// Console.WriteLine(response.Text);
// PrintUsage(response.Usage!);

ChatClientAgent aiAgent = new (chatClient.AsIChatClient(), options);

var fileBytes = await File.ReadAllBytesAsync("Ficha_Producto_NexaSense_XR4.pdf");
var rom = new ReadOnlyMemory<byte>(fileBytes);

var contents = new List<AIContent>();
contents.Add(new TextContent(prompt));
contents.Add(new DataContent(rom, "application/pdf"));

var message = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, contents);

await foreach(AgentResponseUpdate item in aiAgent.RunStreamingAsync(message))
{
    Console.Write(item.Text);

    if (item.Contents.Any() && item.Contents.First() is UsageContent usage)
    {
        PrintUsage(usage.Details);
    }
}

Console.ReadLine();


void PrintUsage(UsageDetails usage)
{
    Console.WriteLine();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Usage:");
    Console.WriteLine($"Total tokens used: {usage.TotalTokenCount}");
    Console.WriteLine($"Input tokens used: {usage.InputTokenCount}");
    Console.WriteLine($"Output tokens used: {usage.OutputTokenCount}");
    Console.WriteLine($"Reasoning tokens used: {usage.ReasoningTokenCount}");
    Console.ForegroundColor = ConsoleColor.Gray;
}