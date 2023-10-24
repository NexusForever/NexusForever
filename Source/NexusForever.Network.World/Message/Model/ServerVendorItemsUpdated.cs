using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerVendorItemsUpdated)]
    public class ServerVendorItemsUpdated : IWritable
    {
        public class Category : IWritable
        {
            public uint Index { get; set; }
            public uint LocalisedTextId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Index);
                writer.Write(LocalisedTextId);
            }
        }

        public class Item : IWritable
        {
            public class ItemExtraCost : IWritable
            {
                public enum ItemExtraCostType : byte
                {
                    None = 0,
                    Item = 1,
                    Currency = 2
                }
                
                public ItemExtraCostType ExtraCostType { get; set; }
                public uint Quantity { get; set; }
                public uint ItemOrCurrencyId { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    writer.Write((byte)ExtraCostType, 3);
                    writer.Write(Quantity);
                    writer.Write(ItemOrCurrencyId);
                }
            }

            public uint Index { get; set; }
            public byte Unknown1 { get; set; }
            public uint ItemId { get; set; }
            public uint Unknown3 { get; set; }
            public uint Unknown4 { get; set; }
            public uint Unknown5 { get; set; }
            public uint Unknown6 { get; set; }
            public uint CategoryIndex { get; set; }
            public uint Unknown8 { get; set; }
            public ulong Unknown9 { get; set; }
            public uint UnknownA { get; set; }
            public ItemExtraCost ExtraCost1 { get; set; }
            public ItemExtraCost ExtraCost2 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Index);
                writer.Write(Unknown1, 4);
                writer.Write(ItemId);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
                writer.Write(Unknown5, 17);
                writer.Write(Unknown6);
                writer.Write(CategoryIndex);
                writer.Write(Unknown8);
                writer.Write(Unknown9);
                writer.Write(UnknownA);
                ExtraCost1.Write(writer);
                ExtraCost2.Write(writer);
            }
        }

        public uint Guid { get; set; }
        public List<Category> Categories { get; } = new();
        public List<Item> Items { get; } = new();
        public float SellPriceMultiplier { get; set; }
        public float BuyPriceMultiplier { get; set; }
        public bool Unknown2 { get; set; }
        public bool Unknown3 { get; set; }
        public bool Unknown4 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);

            writer.Write(Categories.Count);
            Categories.ForEach(c => c.Write(writer));
            writer.Write(Items.Count);
            Items.ForEach(i => i.Write(writer));

            writer.Write(SellPriceMultiplier);
            writer.Write(BuyPriceMultiplier);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
        }
    }
}
