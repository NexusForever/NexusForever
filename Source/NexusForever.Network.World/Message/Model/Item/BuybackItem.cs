using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    public class BuybackItem : IWritable
    {
        public uint UniqueId { get; set; }
        public uint Item2Id { get; set; }
        public uint Quantity { get; set; }
        public CraftStats CircuitData { get; set; } = new();
        public RuneSlots GlyphData { get; set; } = new();
        public ItemThresholds ThresholdData { get; set; } = new();
        public ulong MakerCharacterId { get; set; }
        public uint WorldReq_Item2Id { get; set; }
        public uint[] ChargeAmounts { get; set; } = new uint[5];
        public uint[] GlyphItem2Id_array { get; set; } = new uint[8];
        public ulong CurrencyAmount_First { get; set; }
        public ulong CurrencyAmount_Second { get; set; }
        public CurrencyType CurrencyTypeId_First { get; set; }
        public CurrencyType CurrencyTypeId_Second { get; set; }
        public WorldRequirements WorldRequirements { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UniqueId);
            writer.Write(Item2Id, 18u);
            writer.Write(Quantity);
            CircuitData.Write(writer);
            GlyphData.Write(writer);
            ThresholdData.Write(writer);
            writer.Write(MakerCharacterId);
            writer.Write(WorldReq_Item2Id, 18u);

            for (int i = 0; i < ChargeAmounts.Length; i++)
            {
                writer.Write(ChargeAmounts[i]);
            }

            for (int i = 0; i < GlyphItem2Id_array.Length; i++)
            {
                writer.Write(GlyphItem2Id_array[i]);
            }

            writer.Write(CurrencyAmount_First);
            writer.Write(CurrencyAmount_Second);
            writer.Write(CurrencyTypeId_First, 4u);
            writer.Write(CurrencyTypeId_Second, 4u);

            WorldRequirements.Write(writer);
        }
    }
}
