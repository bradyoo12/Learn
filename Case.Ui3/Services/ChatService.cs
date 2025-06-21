using Azure;
using Azure.AI.OpenAI;
using Case.Ui3.Classes;

namespace Case.Ui3.Services;

public class ChatService : IChatService
{
    //public async Task<ChatCompletions> ChatWithOpenAI(string userMessage)
    //{

    //    var _client = new OpenAIClient(
    //      new Uri("https://seesharpopenai.openai.azure.com/"),
    //      new AzureKeyCredential("[YOUR KEY HERE]"));

    //    Console.WriteLine(userMessage);
    //    Response<ChatCompletions> responseWithoutStream = await _client.GetChatCompletionsAsync(
    //        "seesharp",
    //        new ChatCompletionsOptions()
    //        {
    //            Messages =
    //                        {
    //                new ChatMessage(ChatRole.User, userMessage)
    //                        },
    //            Temperature = (float)0.7,
    //            MaxTokens = 800,
    //            NucleusSamplingFactor = (float)0.95,
    //            FrequencyPenalty = 0,
    //            PresencePenalty = 0
    //        });

    //    return responseWithoutStream.Value;
    //}


    private readonly IHttpClientFactory _httpClientFactory;

    public ChatService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> SendMessageAsync(ChatRequest chatRequest)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AzureAppClient");

            var response = await client.PostAsJsonAsync("/api/Function1", chatRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result ?? "Sorry, I couldn't process your request.";
            }

            return $"Error: {response.StatusCode} {client.BaseAddress}";
        }
        catch (Exception ex)
        {
            return $"Error communicating with the chat service: {ex.Message}";
        }
    }
}


public class ChatResponse
{
    public string Reply { get; set; } = string.Empty;
}
