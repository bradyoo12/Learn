using System;

namespace Case.Chat.Settings
{
    public class AzureOpenAISettings
    {
        public string DeploymentName { get; set; }
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
        public string ModelId { get; set; }
        
        // Additional OpenAI settings
        public float Temperature { get; set; } = 0;
    }
}