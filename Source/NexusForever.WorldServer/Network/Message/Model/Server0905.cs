using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0905, MessageDirection.Server)]
    public class Server0905 : IWritable
    {
        public class UnknownStructure0 : IWritable
        {   
            public byte Unknown0 { get; set; }
            public ushort Unknown1 { get; set; } = 0;
            public ushort Unknown2 { get; set; } = 0;
            public uint Unknown3 { get; set; }


            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 7u);
                writer.Write(Unknown1, 15u);
                writer.Write(Unknown2, 14u);
                writer.Write(Unknown3);
            }
        }
        public uint UnitId { get; set; }
        public byte Unknown0 { get; set; }
        public byte Unknown1 { get; set; }
        public uint CreatureId { get; set; }
        public uint DisplayInfo { get; set; }
        public ushort Unknown4 { get; set; }
        public uint Unknown5 { get; set; }
        public bool Unknown6 { get; set; }
        
        public List<UnknownStructure0> unknownStructure0 { get; set; } = new List<UnknownStructure0>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Unknown0, 5u);
            writer.Write(Unknown1, 2u);
            writer.Write(CreatureId, 18u);
            writer.Write(DisplayInfo, 17u);
            writer.Write(Unknown4, 15);
            writer.Write(Unknown5);
            writer.Write(Unknown6);
            writer.Write(unknownStructure0.Count, 32u);
            unknownStructure0.ForEach(u => u.Write(writer));
        }
    }
}
