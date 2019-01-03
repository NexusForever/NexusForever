using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07FF, MessageDirection.Server)]
    public class Server07FF : IWritable
    {
        public class UnknownStructure0 : IWritable
        {   
            public uint   Unknown0 { get; set; } = 0;
            public byte   Unknown4 { get; set; } = 0;
            public uint   Unknown5 { get; set; } = 0;
            public uint   Unknown9 { get; set; } = 0;
            public uint   Unknown13 { get; set; } = 0;
            public uint   Unknown17 { get; set; } = 0;
            public uint   Unknown21 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown4);
                writer.Write(Unknown5);
                writer.Write(Unknown9);
                writer.Write(Unknown13);
                writer.Write(Unknown17);
                writer.Write(Unknown21);
            }
        }

        public class UnknownStructure1 : IWritable
        {   
            public ushort Unknown0 { get; set; } = 0;
            public uint   Unknown2 { get; set; } = 0;
            public byte   Unknown6 { get; set; } = 0;
            public uint   Unknown7 { get; set; } = 0;
            public uint   Unknown11 { get; set; } = 0;
            public uint   Unknown15 { get; set; } = 0;
            public uint   Unknown19 { get; set; } = 0;
            public uint   Unknown23 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown2);
                writer.Write(Unknown6);
                writer.Write(Unknown7);
                writer.Write(Unknown11);
                writer.Write(Unknown15);
                writer.Write(Unknown19);
                writer.Write(Unknown23);
            }
        }

        public uint CastingId { get; set; }
        public uint Spell4Id { get; set; }
        public uint Spell4Id2 { get; set; }
        public uint Unknown12 { get; set; } = 0;
        public uint Guid { get; set; }
        public ushort Unknown20 { get; set; } = 0;
        public uint Guid2 { get; set; } // target?
        public uint Unknown26 { get; set; } = 0;
        public uint Unknown30 { get; set; } = 0;
        public uint Unknown34 { get; set; } = 0;
        public uint Unknorn38 { get; set; } = 0;        
        public bool Unknown41 { get; set; } = false;
        public bool Unknown42 { get; set; } = false;

        public List<UnknownStructure0> unknownStructure0 { get; set; } = new List<UnknownStructure0>();
        public List<UnknownStructure1> unknownStructure1 { get; set; } = new List<UnknownStructure1>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Spell4Id, 18);
            writer.Write(Spell4Id2, 18);
            writer.Write(Unknown12, 18);
            writer.Write(Guid);
            writer.Write(Unknown20);
            writer.Write(Guid2);
            writer.Write(Unknown26);
            writer.Write(Unknown30);
            writer.Write(Unknown34);
            writer.Write(Unknorn38);

            writer.Write(unknownStructure0.Count, 8u);
            unknownStructure0.ForEach(u => u.Write(writer));

            writer.Write(unknownStructure1.Count, 8u);
            unknownStructure1.ForEach(u => u.Write(writer));

            writer.Write(Unknown41);
            writer.Write(Unknown42);
        }
    }
}
