using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Case.Chat.Settings;
using Microsoft.Extensions.Options;

namespace Case.Chat.Functions
{
    public class BaseFunction
    {
        private readonly AzureOpenAISettings _azureOpenAISettings;
        private static readonly ConcurrentDictionary<string, ChatHistory> _chatHistories = new ConcurrentDictionary<string, ChatHistory>();

        public BaseFunction(IOptions<AzureOpenAISettings> openAISettings)
        {
            _azureOpenAISettings = openAISettings.Value;
        }

        private Kernel CreateKernel()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: _azureOpenAISettings.DeploymentName,
                apiKey: _azureOpenAISettings.ApiKey,
                endpoint: _azureOpenAISettings.Endpoint,
                modelId: _azureOpenAISettings.ModelId
            );

            return builder.Build();
        }

        protected ChatCompletionAgent CreateAgent(string instructions,
            string? description = null,
            string? name = null,
            IEnumerable<KernelFunction> kernelFunctions = null)
        {
            var k = this.CreateKernel();
            if (kernelFunctions != null)
            {
                k.Plugins.AddFromFunctions(name, kernelFunctions);
            }

            return
                new ChatCompletionAgent
                {
                    Name = name,
                    Description = description,
                    Instructions = instructions,
                    Kernel = k,
                    Arguments = new KernelArguments(new OpenAIPromptExecutionSettings()
                    {
                        Temperature = 0,
                        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
                    })
                };
        }
        protected static async Task<ChatRequest> GetChatRequest(HttpRequest req)
        {
            // Read and deserialize the JSON body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<ChatRequest>(requestBody);

            // Get or create conversation ID from arguments
            return requestData;
        }

        /// <summary>
        /// Gets an existing chat history or creates a new one if it doesn't exist
        /// </summary>
        protected ChatHistory GetOrCreateChatHistory(string conversationId)
        {
            return _chatHistories.GetOrAdd(conversationId, _ => new ChatHistory());
        }
    }

    public class ChatRequest
    {
        public string message { get; set; }
        public string conversationId { get; set; }
    }
}
