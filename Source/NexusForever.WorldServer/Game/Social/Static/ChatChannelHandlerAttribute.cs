using System;

namespace NexusForever.WorldServer.Game.Social.Static
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
