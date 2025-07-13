namespace NexusForever.Network.World.Entity.Model
{
    public class PinataLootEntityModel : IEntityModel
    {
        public uint Creature2Id { get; set; }
        public uint ItemId { get; set; } // can be VirtualItem, LootSpell, AccountItem, AccountCurrency, or Item2
        public uint ItemCount { get; set; }
        public byte LootType { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Creature2Id, 18u);
            writer.Write(ItemId);
            writer.Write(ItemCount);
            writer.Write(LootType, 4);
        }
    }
}
