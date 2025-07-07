using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    [Message(GameMessageOpcode.ClientHelloRealm)]
    public class ClientHelloRealm : IReadable
    {
        public uint AccountId { get; private set; }
        public byte[] SessionKey { get; private set; }
        public ulong Unused { get; private set; }
        public string AccountString { get; private set; }
        public uint Always3 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AccountId = reader.ReadUInt();
            SessionKey = reader.ReadBytes(16);
            Unused = reader.ReadULong();
            AccountString = reader.ReadWideString();
            Always3 = reader.ReadUInt();
        }
    }
}
