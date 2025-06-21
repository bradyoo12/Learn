using System.Text.Json;
using System.Text.RegularExpressions;

namespace Case.Core.Tools
{
    public class ChatTool
    {
        public static ChatResponseWithPreviousMessages? BuildChatResponseUsingChatContent(string chatContent)
        {
            ChatResponseWithPreviousMessages chatResponse = null;

            try
            {

                var jsonMatch = Regex.Match(chatContent, @"(\{""AssistantContent"":.*\})", RegexOptions.Singleline);
                var summaryMatch = Regex.Match(chatContent, @"^(?<content>.*?)(?:\\n|\n)?\{""Summary"":""(?<summary>[^""]*)""\}", RegexOptions.Singleline);

                if (jsonMatch.Success)
                {
                    chatContent = jsonMatch.Groups[1].Value;
                    chatResponse = JsonSerializer.Deserialize<ChatResponseWithPreviousMessages>(chatContent);
                }
                else if (summaryMatch.Success)
                {
                    chatResponse = new ChatResponseWithPreviousMessages
                    {
                        AssistantContent = summaryMatch.Groups["content"].Value.Trim(),
                        Summary = summaryMatch.Groups["summary"].Value.Trim()
                    };
                }
                else
                {
                    chatResponse = new ChatResponseWithPreviousMessages
                    {
                        AssistantContent = chatContent,
                        Summary = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                //_logger.LogWarning($"Failed to deserialize ChatResponse: {ex.Message}");
                chatResponse = new ChatResponseWithPreviousMessages
                {
                    AssistantContent = chatContent,
                    Summary = string.Empty
                };
            }

            return chatResponse;
        }
    }


    public class MyChatResponse
    {
        /// <summary>
        /// e.g. 지금부터 한국어로 대화할까요?
        /// </summary>
        public required string AssistantContent { get; set; }
        /// <summary>
        /// e.g. Native Korean speaker
        /// </summary>
        public string Summary { get; set; }
    }

    public class ChatResponseWithPreviousMessages : MyChatResponse
    {
        /// <summary>
        /// e.g. Do you prefer to speak Korean
        /// </summary>
        public string AssistantQuestion { get; set; }
        /// <summary>
        /// e.g. Yes, I am a native Korean
        /// </summary>
        public string UserAnswer { get; set; }
        /// <summary>
        /// e.g. profile-like info: name, job, language
        /// </summary>
        //public bool IsPersonalInformation { get; set; }
        /// <summary>
        /// e.g. sports, health, food, hobby
        /// </summary>
        //public bool IsPersonalInterest { get; set; }

    }
}
