using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07F6, MessageDirection.Server)]
    public class Server07F6 : IWritable
    {
        public class UnknownStructure3 : IWritable
        {
            public uint Unknown0 { get; set; } = 0;
            public uint Unknown1 { get; set; } = 0;
            public uint Unknown2 { get; set; } = 0;
            public uint Unknown3 { get; set; } = 0;
            public uint Unknown4 { get; set; } = 0;
            public uint Unknown5 { get; set; } = 0;
            public uint Unknown6 { get; set; } = 0;
            public byte Unknown7 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
                writer.Write(Unknown5);
                writer.Write(Unknown6);
                writer.Write(Unknown7, 3);
            }
        }

        public class UnknownStructure2 : IWritable // same used for 0x07F4
        {
            public uint Unknown0 { get; set; } = 0;
            public uint Unknown1 { get; set; } = 0;
            public uint Unknown2 { get; set; } = 0;
            public uint Unknown3 { get; set; } = 0;
            public uint Unknown4 { get; set; } = 0;
            public uint Unknown5 { get; set; } = 0;
            public uint Unknown6 { get; set; } = 0;
            public bool Unknown7 { get; set; } = false;
            public byte Unknown8 { get; set; } = 0;
            public byte Unknown9 { get; set; } = 0;
            public List<UnknownStructure3> unknownStructure3 { get; set; } = new List<UnknownStructure3>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
                writer.Write(Unknown5);
                writer.Write(Unknown6);
                writer.Write(Unknown7);
                writer.Write(Unknown8, 4);
                writer.Write(Unknown9, 3);
                
                writer.Write(unknownStructure3.Count, 8u);
                unknownStructure3.ForEach(u => u.Write(writer));
            }
        }

        public uint CastingId { get; set; }
        public uint Spell4EffectId { get; set; } = 0;
        public uint Unknown0 { get; set; } = 0;
        public uint Unknown1 { get; set; } = 0;

        public UnknownStructure2 unknownStructure2 { get; set; } = new UnknownStructure2();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Spell4EffectId, 19);
            writer.Write(Unknown0);
            writer.Write(Unknown1);
            unknownStructure2.Write(writer);
        }
    }
}
