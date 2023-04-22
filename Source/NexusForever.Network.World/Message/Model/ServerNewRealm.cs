﻿using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
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

        public uint Unknown0 { get; set; } // not used in packet handler
        public byte[] SessionKey { get; set; }
        public Gateway GatewayData { get; set; }
        public bool Unknown1C { get; set; } // not used in packet handler
        public string RealmName { get; set; }
        public uint Flags { get; set; }
        public RealmType Type { get; set; }
        public uint NoteTextId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.WriteBytes(SessionKey, 16u);
            GatewayData.Write(writer);
            writer.Write(Unknown1C);
            writer.WriteStringWide(RealmName);
            writer.Write(Flags);
            writer.Write(Type, 2u);
            writer.Write(NoteTextId, 21u);
        }
    }
}
