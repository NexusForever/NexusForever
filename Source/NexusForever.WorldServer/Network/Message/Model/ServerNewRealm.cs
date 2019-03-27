using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerNewRealm, MessageDirection.Server)]
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

        public uint Unknown0 { get; set; } // not used in packet handler
        public byte[] SessionKey { get; set; }
        public Gateway GatewayData { get; set; }
        public bool Unknown1C { get; set; } // not used in packet handler
        public string RealmName { get; set; }
        public uint Unknown24 { get; set; }
        public RealmType Type { get; set; }
        public uint Unknown2C { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.WriteBytes(SessionKey, 16u);
            GatewayData.Write(writer);
            writer.Write(Unknown1C);
            writer.WriteStringWide(RealmName);
            writer.Write(Unknown24);
            writer.Write(Type, 2u);
            writer.Write(Unknown2C, 21u);
        }
    }
}
