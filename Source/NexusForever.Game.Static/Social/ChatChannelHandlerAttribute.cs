namespace NexusForever.Game.Static.Social
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ChatChannelHandlerAttribute : Attribute
    {
        public ChatChannelType ChatChannel { get; }

        public ChatChannelHandlerAttribute(ChatChannelType chatChannel)
        {
            ChatChannel = chatChannel;
        }
    }
}
