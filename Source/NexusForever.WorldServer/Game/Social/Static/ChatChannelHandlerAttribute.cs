using System;

namespace NexusForever.WorldServer.Game.Social.Static
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ChatChannelHandlerAttribute : Attribute
    {
        public ChatChannel ChatChannel { get; }

        public ChatChannelHandlerAttribute(ChatChannel chatChannel)
        {
            ChatChannel = chatChannel;
        }
    }
}
