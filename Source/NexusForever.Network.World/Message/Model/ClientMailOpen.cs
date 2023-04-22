using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMailOpen)]
    public class ClientMailOpen : IReadable
    {
        public List<ulong> MailList { get; } = new();

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (int i = 0; i < count; i++)
                MailList.Add(reader.ReadULong());
        }
    }
}
