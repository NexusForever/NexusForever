using NexusForever.Game.Static.Social;

namespace NexusForever.Network.World.Social
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
