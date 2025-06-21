using Case.Core.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Functions;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Case.Chat.Settings;

namespace Case.Chat.Functions;

public class Function1 : BaseFunction
{
    // Dictionary to store chat histories by sessionId
    private static readonly Dictionary<string, ChatHistory> _chatHistories = new Dictionary<string, ChatHistory>();

    public Function1(IOptions<AzureOpenAISettings> openAISettings) : base(openAISettings)
    {
    }

    [Function("Function1")]
    public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        // Read the request body

        var chatRequest = await GetChatRequest(req);

        if (chatRequest == null)
        {
            return new BadRequestObjectResult("Please provide a valid request with 'message' field");
        }

        // Get or create chat history for this conversation
        ChatHistory chatHistory = GetOrCreateChatHistory(chatRequest.conversationId);

        var instructions = $"""
                You are a helpful assistant that confirms the user's request. 
                You first call any kernel function without isConfirmed, then if the user confirms, isConfirmed is true.
                """;

        var buySharesFunction = KernelFunctionFactory.CreateFromMethod(BuyShares);
        var agent = this.CreateAgent(instructions, "Stock Agent", "StockAgent", new[] { buySharesFunction });

        // Collect agent responses
        var responses = new List<string>();

        // Add user message to chat history
        chatHistory.AddUserMessage(chatRequest.message);

        ChatMessageContent message = await agent.InvokeAsync(chatHistory).FirstAsync();

        return new OkObjectResult(message.Content);
    }

    [Description("Buy a number of shares")]
    [KernelFunction]
    public string BuyShares(
            [Description("the number of shares to buy")] int numberOfShares,
            [Description("the name of shares")] string nameOfShares,
            [Description("is confirmed")] bool isConfirmed = false)
    {
        // Simple validation logic
        if (string.IsNullOrEmpty(nameOfShares))
        {
            return "Request is invalid. The name of shares is required.";
        }

        return isConfirmed ?
            $"You have asked me to buy {numberOfShares} share(s) of {nameOfShares}. Would you proceed?" :
            $"You have bought {numberOfShares} share(s) of {nameOfShares}";
    }
}
