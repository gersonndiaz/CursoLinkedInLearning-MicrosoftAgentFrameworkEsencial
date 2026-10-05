using System.ComponentModel;
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
// // var prompt = "Cuál es el presupuesto máximo para hoteles en Copiapó?";
// var prompt = "Cuál es el presupuesto máximo de viáticos por día y para unan noche de hotel para Copiapó?";
string? prompt = null;

// Versión antigua del curso
// var fileAgentSkillsProvider = new FileAgentSkillsProvider(Path.Combine(AppContext.BaseDirectory, "skills"));

// Versión nueva 
// var skillsProvider = new AgentSkillsProvider(
//     Path.Combine(AppContext.BaseDirectory, "skills"));
// En esta versión se deshabilitan las aprobaciones de carga de skills y lectura de recursos de skills para simplificar el ejemplo. En un entorno de producción, se recomienda habilitar estas aprobaciones para mayor seguridad y control.
var skillsProvider = new AgentSkillsProvider(
    Path.Combine(AppContext.BaseDirectory, "skills"),
    options: new AgentSkillsProviderOptions
    {
        DisableLoadSkillApproval = true,
        DisableReadSkillResourceApproval = true
    });


ChatClientAgentOptions options = new()
{
    AIContextProviders = [ skillsProvider ],
    ChatOptions = new()
    {
        Instructions = """
        Eres un agente que ayuda a responder pregutas acerca de las políticas de viaje de la empresa.
        No contestes nada relacionado a otra cosa.
        No sugieras nada más allá de lo que se te pide.
        Contesta de manera concisa y clara.
        Usa el español para responder.
        Usa única y exclusivamente las herramientas que tengas disponibles para responder a las preguntas.
        """,
        MaxOutputTokens = 1000,
        Reasoning = new ReasoningOptions { Effort = ReasoningEffort.Low }
        ,
        Tools = [ AIFunctionFactory.Create(GetHotelMaxBudget), AIFunctionFactory.Create(GetAllowancePerDay) ]
    }
};

ChatClientAgent aiAgent = new (chatClient.AsIChatClient(), options);

// Antes de integrar las skills era como se encuentra comentado
// var contents = new List<AIContent>
// {
//     new TextContent(prompt)
// };

// var message = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, contents);

// await foreach(AgentResponseUpdate item in aiAgent.RunStreamingAsync(message))
// {
//     Console.Write(item.Text);

//     if (item.Contents.Any() && item.Contents.First() is UsageContent usage)
//     {
//         PrintUsage(usage.Details);
//     }
    
//     if (item.Contents.Any() && item.Contents.First() is FunctionCallContent functionCall)
//     {
//         Console.WriteLine();
//         Console.ForegroundColor = ConsoleColor.Green;
//         foreach (FunctionCallContent fcc in item.Contents)
//         {
//             Console.WriteLine($"Function called: {fcc.Name}");
//             foreach (var arg in fcc.Arguments!)
//             {
//                 Console.WriteLine($"Argument: {arg.Key} = {arg.Value}");
//             }
//         }
//         Console.ForegroundColor = ConsoleColor.Gray;
//     }

//     if (item.Contents.Any() && item.Contents.First() is FunctionResultContent functionResult)
//     {
//         Console.WriteLine();
//         Console.ForegroundColor = ConsoleColor.Cyan;
//         foreach (FunctionResultContent frc in item.Contents)
//         {
//             Console.WriteLine($"Function result: {frc.Result}");
//         }
//         Console.ForegroundColor = ConsoleColor.Gray;
//     }
// }

while(true)
{
    Console.WriteLine("Escribe tu pregunta:");
    prompt = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(prompt) || prompt.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var contents = new List<AIContent>
    {
        new TextContent(prompt)
    };

    var message = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, contents);

    await foreach(AgentResponseUpdate item in aiAgent.RunStreamingAsync(message))
    {
        Console.Write(item.Text);

        if (item.Contents.Any() && item.Contents.First() is UsageContent usage)
        {
            PrintUsage(usage.Details);
        }
        
        if (item.Contents.Any() && item.Contents.First() is FunctionCallContent functionCall)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (FunctionCallContent fcc in item.Contents)
            {
                Console.WriteLine($"Function called: {fcc.Name}");
                foreach (var arg in fcc.Arguments!)
                {
                    Console.WriteLine($"Argument: {arg.Key} = {arg.Value}");
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        if (item.Contents.Any() && item.Contents.First() is FunctionResultContent functionResult)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (FunctionResultContent frc in item.Contents)
            {
                Console.WriteLine($"Function result: {frc.Result}");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
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

Money GetHotelMaxBudget(string city)
{
    return city.Equals("Copiapó", StringComparison.InvariantCultureIgnoreCase) ? new Money(100000) : new Money(200000);
}

[Description("Regresa el presupuesto máximo de viáticos por día para la ciudad especificada.")]
Money GetAllowancePerDay([Description("El nombre de la ciudad")] string city)
{
    return city.Equals("Copiapó", StringComparison.InvariantCultureIgnoreCase) ? new Money(100) : new Money(300);
}

record Money(decimal Amount, string Currency = "CLP");