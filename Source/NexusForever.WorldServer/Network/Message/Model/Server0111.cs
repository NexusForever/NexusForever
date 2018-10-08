using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0111, MessageDirection.Server)]
    public class Server0111 : IWritable
    {
        #region UnknownStructures

        public class UnknownStructure0 : IWritable
        {
            public ushort Unknown0 { get; set; }
            public uint Slot { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 9); // 4
                writer.Write(Slot);
            }
        }

        public class UnknownStructure1 : IWritable
        {
            public byte Unknown0 { get; set; }
            public uint Unknown4 { get; set; }
            public uint Unknown8 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 3);
                writer.Write(Unknown4);
                writer.Write(Unknown8);
            }
        }

        public class UnknownStructure2 : IWritable
        {
            public ushort Unknown0 { get; set; }
            public ulong Unknown8 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 14);
                writer.Write(Unknown8);
            }
        }

        #endregion

        public ulong Unknown0 { get; set; }
        public ulong Unknown8 { get; set; }
        public uint Id { get; set; }
        public UnknownStructure0 Unknown14 { get; } = new UnknownStructure0();
        public uint Unknown1C { get; set; }
        public uint Unknown20 { get; set; }
        public ulong Unknown28 { get; set; }
        public uint Unknown30 { get; set; }
        public ulong Unknown38 { get; set; }
        public uint Unknown40 { get; set; }
        public uint Unknown44 { get; set; }
        public byte Unknown48 { get; set; }
        public uint Unknown4C { get; set; }
        public uint Unknown50 { get; set; }
        public uint Unknown54 { get; set; }
        public UnknownStructure1[] Unknown58 { get; set; } = new UnknownStructure1[2];
        public uint Unknown74 { get; set; }
        public List<uint> Unknown78 { get; } = new List<uint>();
        public List<uint> Unknown80 { get; } = new List<uint>();
        public List<UnknownStructure2> Unknown88 { get; } = new List<UnknownStructure2>();
        public uint Unknown8C { get; set; }
        public byte Unknown90 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Unknown8);
            writer.Write(Id, 18);

            Unknown14.Write(writer);

            writer.Write(Unknown1C);
            writer.Write(Unknown20);
            writer.Write(Unknown28);
            writer.Write(Unknown30);
            writer.Write(Unknown38);
            writer.Write(Unknown40);
            writer.Write(Unknown44);
            writer.Write(Unknown48);
            writer.Write(Unknown4C);
            writer.Write(Unknown50);
            writer.Write(Unknown54);

            for (uint i = 0u; i < 2u; i++)
                Unknown58[i].Write(writer);

            writer.Write(Unknown74, 18);

            writer.Write((byte)Unknown78.Count, 3);
            Unknown78.ForEach(i => writer.Write(i));
            writer.Write((byte)Unknown80.Count, 4);
            Unknown80.ForEach(i => writer.Write(i));
            writer.Write((byte)Unknown88.Count, 6);
            Unknown88.ForEach(s => s.Write(writer));

            writer.Write(Unknown8C);

            // 49 = item
            // !49 = ability
            writer.Write(Unknown90, 6);
        }
    }
}
