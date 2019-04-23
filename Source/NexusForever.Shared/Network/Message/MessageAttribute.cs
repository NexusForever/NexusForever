using System;

namespace NexusForever.Shared.Network.Message
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public GameMessageOpcode Opcode { get; }

        public MessageAttribute(GameMessageOpcode opcode)
        {
            Opcode = opcode;
        }
    }
}
