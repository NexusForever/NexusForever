using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Item
{
    public class Item : IWritable
    {
        public class PriceInfo : IWritable
        {
            public byte CostType { get; set; }
            public uint Amount { get; set; }
            public uint AltAmount { get; set; }
            public uint CurrencyId { get; set; }
            public uint Value { get; set; } // can be a currency type or a secondary currency value depending on CostType

            public void Write(GamePacketWriter writer)
            {
                writer.Write(CostType, 3u);
                writer.Write(Amount);
                writer.Write(Value);
            }
        }

        public ulong ItemGuid { get; set; }
        public ulong MakerCharacterId { get; set; }
        public uint Item2Id { get; set; }
        public ItemLocation LocationData { get; set; }
        public uint StackCount { get; set; }
        public uint Charges { get; set; }
        public CraftStats CircuitData { get; set; } = new ();
        public RuneSlots GlyphData { get; set; } = new ();
        public ItemThresholds ThresholdData { get; set; } = new ();
        public float Durability { get; set; }
        public uint Unknown44 { get; set; } // possibly costume related
        public byte Flags { get; set; }
        public uint ReturnTimeRemaining { get; set; }
        public uint ExpireTimeRemaining { get; set; }
        public uint UpdateTimeOffset { get; set; }
        public PriceInfo SellPrice_Primary { get; set; }
        public PriceInfo SellPrice_Secondary { get; set; }
        public uint PowerCoreItem2Id { get; set; }
        public List<uint> Microchips { get; } = new(); // max 5
        public List<uint> Glyphs { get; } = new(); // max 6
        public List<Identity> TimeLimitedTradingPartnerIdentities { get; } = [];
        public WorldRequirements WorldRequirements { get; set; } = new ();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(MakerCharacterId);
            writer.Write(Item2Id, 18u);
            LocationData.Write(writer);
            writer.Write(StackCount);
            writer.Write(Charges);
            CircuitData.Write(writer);
            GlyphData.Write(writer);
            ThresholdData.Write(writer);
            writer.Write(Durability);
            writer.Write(Unknown44);
            writer.Write(Flags);
            writer.Write(ReturnTimeRemaining);
            writer.Write(ExpireTimeRemaining);
            writer.Write(UpdateTimeOffset);

            SellPrice_Primary.Write(writer);
            SellPrice_Secondary.Write(writer);

            writer.Write(PowerCoreItem2Id, 18u);
            writer.Write(Microchips.Count, 3u);
            Microchips.ForEach(m => writer.Write(m));

            writer.Write(Glyphs.Count, 4u);
            Glyphs.ForEach(g => writer.Write(g));

            writer.Write(TimeLimitedTradingPartnerIdentities.Count, 6u);
            TimeLimitedTradingPartnerIdentities.ForEach(u => u.Write(writer));

            WorldRequirements.Write(writer);
        }
    }
}
