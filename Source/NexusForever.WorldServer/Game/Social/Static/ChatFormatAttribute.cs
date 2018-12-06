using System;

namespace NexusForever.WorldServer.Game.Social.Static
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ChatFormatAttribute : Attribute
    {
        public ChatFormatType Type;

        public ChatFormatAttribute(ChatFormatType type)
        {
            Type = type;
        }
    }
}
