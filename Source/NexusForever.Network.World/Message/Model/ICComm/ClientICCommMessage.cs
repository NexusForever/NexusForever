using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientICCommMessage)]
    public class ClientICCommMessage : IReadable
    {
        public ulong IccommId { get; private set; }
        public uint MessageId { get; private set; }
        public string Message { get; private set; }
        public string RecipientName { get; private set; } // only sent for private messages

        public void Read(GamePacketReader reader)
        {
            IccommId = reader.ReadULong();
            MessageId = reader.ReadUInt();
            Message = reader.ReadStringChar();
            RecipientName = reader.ReadWideString();
        }
    }
}
