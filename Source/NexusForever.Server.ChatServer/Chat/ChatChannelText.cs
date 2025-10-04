namespace NexusForever.Server.ChatServer.Chat
{
    public class ChatChannelText
    {
        public string Text { get; set; }
        public List<ChatChannelTextFormat> Format { get; set; } = [];
    }
}
