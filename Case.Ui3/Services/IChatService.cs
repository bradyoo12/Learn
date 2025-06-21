using Azure.AI.OpenAI;
using Case.Ui3.Classes;

namespace Case.Ui3.Services;

public interface IChatService
{
    //Task<ChatCompletions> ChatWithOpenAI(string userMessage);
    Task<string> SendMessageAsync(ChatRequest chatRequest);
}
