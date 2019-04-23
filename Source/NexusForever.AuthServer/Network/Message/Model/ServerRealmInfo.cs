using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmInfo)]
    public class ServerRealmInfo : IWritable
    {
        public uint Address { get; set; }
        public ushort Port { get; set; }
        public byte[] SessionKey { get; set; }
        public uint AccountId { get; set; }
        public string Realm { get; set; }
        public uint Flags { get; set; }
        public byte Type { get; set; }
        public uint NoteTextId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Address);
            writer.Write(Port);
            writer.WriteBytes(SessionKey, 16u);
            writer.Write(AccountId);
            writer.WriteStringWide(Realm);
            writer.Write(Flags);
            writer.Write(Type, 2);
            writer.Write(NoteTextId, 21u);
        }
    }
}
