using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server06BA, MessageDirection.Server)]
    public class Server06BA : IWritable
    {
        public class UnknownStructure : IWritable
        {
            public uint Unknown0 { get; set; }
            public bool Unknown1 { get; set; }
            public uint Unknown2 { get; set; }
            public uint Unknown3 { get; set; }
            public byte Unknown4 { get; set; }
            public uint Unknown5 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 15);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Unknown4, 3);
                writer.Write(Unknown5);
            }
        }

        public List<UnknownStructure> UnknownStructures { get; set; } = new List<UnknownStructure>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnknownStructures.Count);
            UnknownStructures.ForEach(e => e.Write(writer));
        }
    }
}
