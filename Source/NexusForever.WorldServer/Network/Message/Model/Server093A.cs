using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server093A)]
    public class Server093A : IWritable
    {
        public class UnknownStructure0 : IWritable
        {   
            public byte Unknown0 { get; set; }
            public uint Unknown1 { get; set; } = 0; // something Spell4Effects databit00 related
            public uint Unknown2 { get; set; } = 0; // something Spell4Effects databit00 related

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
            }
        }

        public uint UnitId { get; set; }
        
        public List<UnknownStructure0> unknownStructure0 { get; set; } = new List<UnknownStructure0>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(unknownStructure0.Count, 8u);
            unknownStructure0.ForEach(u => u.Write(writer));
        }
    }
}
