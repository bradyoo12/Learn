namespace Case.Ui3.Classes
{

    public class ChatRequest
    {
        public string message { get; set; }
        public string conversationId { get; set; }
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
