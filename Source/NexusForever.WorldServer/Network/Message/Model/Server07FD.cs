using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07FD, MessageDirection.Server)]
    public class Server07FD : IWritable
    {
        public class UnknownStructure0 : IWritable
        {
            public uint   Unknown0 { get; set; } = 0;
            public byte   Unknown4 { get; set; } = 0;
            public Position Position { get; set; } = new Position();
            public float  Unknown16 { get; set; } = 0;
            public uint   Unknown20 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown4);
                Position.Write(writer);
                writer.Write(Unknown16);
                writer.Write(Unknown20);
            }
        }

        public class UnknownStructure1 : IWritable
        {
            public ushort Unknown0 { get; set; } = 0;
            public uint   Unknown2 { get; set; } = 0;
            public byte   Unknown6 { get; set; } = 0;
            public Position Position { get; set; } = new Position();
            public float  Unknown18 { get; set; } = 0;
            public uint   Unknown22 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown2);
                writer.Write(Unknown6);
                Position.Write(writer);
                writer.Write(Unknown18);
                writer.Write(Unknown22);
            }
        }

        public uint Unknown0 { get; set; }
        public uint CastingId { get; set; }
        public uint CasterId { get; set; }
        public Position Position { get; set; } = new Position();
        public uint Unknown16 { get; set; }

        public List<UnknownStructure0> unknownStructure0 { get; set; } = new List<UnknownStructure0>();
        public List<UnknownStructure1> unknownStructure1 { get; set; } = new List<UnknownStructure1>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(CastingId);
            writer.Write(CasterId);
            Position.Write(writer);
            writer.Write(Unknown16);

            writer.Write(unknownStructure0.Count, 8u);
            unknownStructure0.ForEach(u => u.Write(writer));

            writer.Write(unknownStructure1.Count, 8u);
            unknownStructure1.ForEach(u => u.Write(writer));
        }
    }
}
