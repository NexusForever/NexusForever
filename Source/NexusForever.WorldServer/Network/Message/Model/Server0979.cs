using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountItemsPending)]
    public class Server0979 : IWritable
    {
        public class UnknownStructure0: IWritable
        {
            public ulong Unknown0 { get; set; }
            public uint Unknown1 { get; set; }
            public ulong Unknown2 { get; set; }
            public string Unknown3 { get; set; }
            public uint Unknown4 { get; set; }
            public ushort Unknown5 { get; set; } // 14
            public ulong Unknown6 { get; set; }
            public ulong Unknown7 { get; set; }
            public byte Unknown8 { get; set; } // 5
            public ulong Unknown9 { get; set; }
            public ushort Unknown10 { get; set; } // 14
            public ulong Unknown11 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.WriteStringWide(Unknown3);
                writer.Write(Unknown4);
                writer.Write(Unknown5, 14u);
                writer.Write(Unknown6);
                writer.Write(Unknown7);
                writer.Write(Unknown8, 5u);
                writer.Write(Unknown9);
                writer.Write(Unknown10);
                writer.Write(Unknown11);
            }
        }

        public List<UnknownStructure0> UnknownStructureList { get; set; } = new List<UnknownStructure0>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnknownStructureList.Count);
            UnknownStructureList.ForEach(w => w.Write(writer));
        }
    }
}
