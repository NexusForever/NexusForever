using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStoreCategories)]
    public class ServerStoreCategories : IWritable
    {
        public class StoreCategory : IWritable
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public uint CategoryId { get; set; } // Id of this category
            public uint ParentCategoryId { get; set; }
            public uint Index { get; set; } // Seems to be indexed from 1
            public bool Visible { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(Name);
                writer.WriteStringWide(Description);
                writer.Write(CategoryId);
                writer.Write(ParentCategoryId);
                writer.Write(Index);
                writer.Write(Visible);
            }
        }

        public class CurrencyPackage : IWritable
        {
            public uint Id { get; set; }
            public string Name { get; set; }
            public uint Count { get; set; }
            public float Price { get; set; }
            public uint Unknown9 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Id);
                writer.WriteStringWide(Name);
                writer.Write(Count);
                writer.Write(Price);
                writer.Write(Unknown9);
            }
        }

        public List<StoreCategory> StoreCategories { get; set; } = new List<StoreCategory>();
        public RealCurrency RealCurrency { get; set; }
        public List<CurrencyPackage> CurrencyPackages { get; set; } = new List<CurrencyPackage>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(StoreCategories.Count);
            StoreCategories.ForEach(e => e.Write(writer));

            writer.Write(RealCurrency, 3);

            writer.Write(CurrencyPackages.Count);
            CurrencyPackages.ForEach(e => e.Write(writer));
        }
    }
}
