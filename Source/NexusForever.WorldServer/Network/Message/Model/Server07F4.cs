using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07F4, MessageDirection.Server)]
    public class Server07F4 : IWritable
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
                writer.Write(Unknown7, 3u);
            }
        }

        public class UnknownStructure2 : IWritable // same used for 0x07F6
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
                writer.Write(Unknown8, 4u);
                writer.Write(Unknown9, 3u);
                
                writer.Write(unknownStructure3.Count, 8u);
                unknownStructure3.ForEach(u => u.Write(writer));
            }
        }

        public class UnknownStructure1 : IWritable
        {
            public uint Spell4EffectId { get; set; } = 0;
            public uint Unknown0 { get; set; } = 0;
            public uint Unknown1 { get; set; } = 0;
            public uint Unknown2 { get; set; } = 0;
            public byte InfoType { get; set; } = 0;

            public List<UnknownStructure1> unknownStructure1 { get; set; } = new List<UnknownStructure1>();
            public UnknownStructure2 unknownStructure2 { get; set; } = new UnknownStructure2();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Spell4EffectId, 19u);
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(InfoType, 2u);
                
                if (InfoType == 1)
                    unknownStructure2.Write(writer);
                else
                    writer.Write(0u, 1u);
            }
        }

        public class UnknownStructure0 : IWritable // same used for 0x0818
        {
            public uint   TargetId { get; set; } = 0; // or Target
            public byte   Unknown1 { get; set; } = 0;
            public byte   Unknown2 { get; set; } = 0;
            public ushort Unknown3 { get; set; } = 0;
            public byte   Unknown4 { get; set; } = 0;

            public List<UnknownStructure1> unknownStructure1 { get; set; } = new List<UnknownStructure1>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(TargetId);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Unknown4, 4u);

                writer.Write(unknownStructure1.Count, 8u);
                unknownStructure1.ForEach(u => u.Write(writer));
            }
        }

        public class UnknownStructure4 : IWritable
        {
            public uint     Unknown0 { get; set; } = 0;
            public byte     Unknown1 { get; set; } = 0;
            public Position Position { get; set; } = new Position();
            public uint     Unknown3 { get; set; } = 0;
            public uint     Unknown4 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                Position.Write(writer);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
            }
        }

        public class UnknownStructure5 : IWritable
        {
            public ushort   Unknown0 { get; set; } = 0;
            public uint     Unknown1 { get; set; } = 0;
            public byte     Unknown2 { get; set; } = 0;
            public Position Position { get; set; } = new Position();
            public uint     Unknown3 { get; set; } = 0;
            public uint     Unknown4 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                Position.Write(writer);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
            }
        }

        public class UnknownStructure6 : IWritable
        {
            public Position Position0 { get; set; } = new Position();
            public uint     Unknown0 { get; set; } = 0;
            public uint     Unknown1 { get; set; } = 0;
            public Position Position1 { get; set; } = new Position();
            public bool     Unknown2  { get; set; } = false;

            public void Write(GamePacketWriter writer)
            {
                Position0.Write(writer);
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                Position1.Write(writer);
                writer.Write(Unknown2);
            }
        }

        public uint CastingId { get; set; }
        public bool Unknown0  { get; set; } = false;
        public Position Position { get; set; } = new Position();

        public List<UnknownStructure0> unknownStructure0 { get; set; } = new List<UnknownStructure0>();
        public List<UnknownStructure4> unknownStructure4 { get; set; } = new List<UnknownStructure4>();
        public List<UnknownStructure5> unknownStructure5 { get; set; } = new List<UnknownStructure5>();
        public List<UnknownStructure6> unknownStructure6 { get; set; } = new List<UnknownStructure6>();

        public byte Unknown1 { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Unknown0);
            Position.Write(writer);

            writer.Write(unknownStructure0.Count, 8u);
            unknownStructure0.ForEach(u => u.Write(writer));

            writer.Write(unknownStructure4.Count, 8u);
            unknownStructure4.ForEach(u => u.Write(writer));

            writer.Write(unknownStructure5.Count, 8u);
            unknownStructure5.ForEach(u => u.Write(writer));

            writer.Write(unknownStructure6.Count, 8u);
            unknownStructure6.ForEach(u => u.Write(writer));

            writer.Write(Unknown1);
        }
    }
}
