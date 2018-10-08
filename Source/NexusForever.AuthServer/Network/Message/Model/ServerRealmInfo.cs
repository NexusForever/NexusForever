using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmInfo, MessageDirection.Server)]
    public class ServerRealmInfo : IWritable
    {
        public uint Host { get; set; }
        public ushort Port { get; set; }
        public byte[] SessionKey { get; set; }
        public uint AccountId { get; set; }
        public string Realm { get; set; }
        public uint Unknown1 { get; set; }
        public byte Type { get; set; }
        public uint Unknown3 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Host);
            writer.Write(Port);
            writer.WriteBytes(SessionKey, 16u);
            writer.Write(AccountId);
            writer.WriteStringWide(Realm);
            writer.Write(Unknown1);
            writer.Write(Type, 2);
            writer.Write(Unknown3, 21);
        }
    }
}
