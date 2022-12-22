namespace NexusForever.Network.Message
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        public GameMessageOpcode Opcode { get; }

        public MessageHandlerAttribute(GameMessageOpcode opcode)
        {
            Opcode = opcode;
        }
    }
}
