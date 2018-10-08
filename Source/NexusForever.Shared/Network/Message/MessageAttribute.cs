using System;

namespace NexusForever.Shared.Network.Message
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public GameMessageOpcode Opcode { get; }
        public MessageDirection Direction { get; }

        public MessageAttribute(GameMessageOpcode opcode, MessageDirection direction)
        {
            Opcode    = opcode;
            Direction = direction;
        }
    }
}
