using NexusForever.Game.Static.Pregame;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Client only processes message when on RealmSelect screen.
    [Message(GameMessageOpcode.ServerNewRealm)]
    public class ServerNewRealm : IWritable
    {
        public class Gateway : IWritable
        {
            public uint Address { get; set; }
            public ushort Port { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Address);
                writer.Write(Port);
            }
        }

        public uint Unused { get; set; }
        public byte[] SessionKey { get; set; }
        public Gateway GatewayData { get; set; }
        public bool Unused2 { get; set; }
        public string RealmName { get; set; }
        public RealmFlag Flags { get; set; }
        public RealmType Type { get; set; }
        public uint NoteTextId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused);
            writer.WriteBytes(SessionKey, 16u);
            GatewayData.Write(writer);
            writer.Write(Unused2);
            writer.WriteStringWide(RealmName);
            writer.Write(Flags, 32u);
            writer.Write(Type, 2u);
            writer.Write(NoteTextId, 21u);
        }
    }
}
