using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientMailSend, MessageDirection.Client)]
    public class ClientMailSend : IReadable
    {
        public string Name { get; private set; }
        public string Realm { get; private set; }
        public string Subject { get; private set; }
        public string Message { get; private set; }
        public ulong Unknown4 { get; private set; }
        public ulong Unknown5 { get; private set; }
        public byte Unknown6 { get; private set; }
        public uint Unknown7 { get; private set; }
        public ulong Unknown8 { get; private set; }


        public void Read(GamePacketReader reader)
        {
            Name = reader.ReadWideString();
            Realm = reader.ReadWideString();
            Subject = reader.ReadWideString();
            Message = reader.ReadWideString();
            Unknown4 = reader.ReadULong();
            Unknown5 = reader.ReadULong();
            Unknown6 = reader.ReadByte(2);
            Unknown7 = reader.ReadUInt();
            Unknown8 = reader.ReadULong();
        }
    }
}
