using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStoreCategories)]
    public class ServerStoreCategories : IWritable
    {
        public class StoreCategory : IWritable
        {
            public string CategoryName { get; set; }
            public string CategoryDesc { get; set; }
            public uint CategoryId { get; set; } // Id of this category
            public uint ParentCategoryId { get; set; } // 26 is "Top Level"
            public uint Index { get; set; } // Seems to be indexed from 1
            public bool Visible { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(CategoryName);
                writer.WriteStringWide(CategoryDesc);
                writer.Write(CategoryId);
                writer.Write(ParentCategoryId);
                writer.Write(Index);
                writer.Write(Visible);
            }
        }

        public class UnknownStructure0 : IWritable
        {
            public uint Unknown5 { get; set; }
            public string Unknown6 { get; set; }
            public uint Unknown7 { get; set; }
            public uint Unknown8 { get; set; }
            public uint Unknown9 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown5);
                writer.WriteStringWide(Unknown6);
                writer.Write(Unknown7);
                writer.Write(Unknown8);
                writer.Write(Unknown9);
            }
        }

        public List<StoreCategory> StoreCategories { get; set; } = new List<StoreCategory>();
        public byte Unknown4 { get; set; }
        public List<UnknownStructure0> Unknown10 { get; set; } = new List<UnknownStructure0>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(StoreCategories.Count);
            StoreCategories.ForEach(e => e.Write(writer));

            writer.Write(Unknown4, 3);

            writer.Write(Unknown10.Count);
            Unknown10.ForEach(e => e.Write(writer));
        }
    }
}
