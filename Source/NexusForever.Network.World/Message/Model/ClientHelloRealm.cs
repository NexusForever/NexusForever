using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHelloRealm)]
    public class ClientHelloRealm : IReadable
    {
        public uint AccountId { get; private set; }
        public byte[] SessionKey { get; private set; }
        public ulong Unknown18 { get; private set; }
        public string Email { get; private set; }
        public uint Unknown24 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountId  = reader.ReadUInt();
            SessionKey = reader.ReadBytes(16);
            Unknown18  = reader.ReadULong();
            Email      = reader.ReadWideString();
            Unknown24  = reader.ReadUInt();
        }
    }
}
