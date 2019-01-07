using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07F8, MessageDirection.Server)]
    public class Server07F8 : IWritable
    {
        public class UnknownStructure0 : IWritable
        {
            public class UnknownStructure1 : IWritable
            {
                public uint Unknown0 { get; set; } = 0;
                public uint Unknown4 { get; set; } = 0;
                public uint Unknown8 { get; set; } = 0;
                public uint Unknown12 { get; set; } = 0;
                public uint Unknown16 { get; set; } = 0;
                public uint Unknown20 { get; set; } = 0;
                public uint Unknown24 { get; set; } = 0;
                public byte Unknown25 { get; set; } = 0;
                public void Write(GamePacketWriter writer)
                {
                    writer.Write(Unknown0);
                    writer.Write(Unknown4);
                    writer.Write(Unknown8);
                    writer.Write(Unknown12);
                    writer.Write(Unknown16);
                    writer.Write(Unknown20);
                    writer.Write(Unknown24);
                    writer.Write(Unknown25);
                }
            }

            public uint Unknown0 { get; set; } = 0;
            public uint Unknown4 { get; set; } = 0;
            public uint Unknown8 { get; set; } = 0;
            public uint Unknown12 { get; set; } = 0;
            public uint Unknown16 { get; set; } = 0;
            public uint Unknown20 { get; set; } = 0;
            public uint Unknown24 { get; set; } = 0;
            public bool Unknown25 { get; set; } = false;
            public byte Unknown26 { get; set; } = 0;
            public byte Unknown27 { get; set; } = 0;

            public List<UnknownStructure1> unknownStructure1 { get; set; } = new List<UnknownStructure1>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown4);
                writer.Write(Unknown8);
                writer.Write(Unknown12);
                writer.Write(Unknown16);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown25);
                writer.Write(Unknown26);
                writer.Write(Unknown27);

                writer.Write(unknownStructure1.Count, 8u);
                unknownStructure1.ForEach(u => u.Write(writer));
            }
        }

        public uint CastingId { get; set; }
        public uint Spell4EffectId { get; set; }
        public uint CasterId { get; set; }

        public List<UnknownStructure0> unknownStructure0 { get; set; } = new List<UnknownStructure0>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Spell4EffectId, 19);
            writer.Write(CasterId);

            writer.Write(unknownStructure0.Count, 8u);
            unknownStructure0.ForEach(u => u.Write(writer));
        }
    }
}
