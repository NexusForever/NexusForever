using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Loot
{
    public class LootItem : IWritable
    {
        public uint LootUnitId { get; set; }
        public LootItemType Type { get; set; }
        public uint ItemId { get; set; }
        public uint Amount { get; set; }
        public bool CanLoot { get; set; }
        public bool RequiresRoll { get; set; }
        public bool OnlyMasterLootable { get; set; }
        public bool Explosion { get; set; }
        public uint RollTime { get; set; }
        public ulong RandomCircuitData { get; set; }
        public uint RandomGlyphData { get; set; }
        public uint ItemQuality2Id { get; set; }
        public List<Identity> MasterList { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LootUnitId);
            writer.Write(Type, 32u);
            writer.Write(ItemId); // Can be Item2Id, LootSpellId, VirtualItemId, AccountItemId
            writer.Write(Amount);
            writer.Write(CanLoot);
            writer.Write(RequiresRoll);
            writer.Write(OnlyMasterLootable);
            writer.Write(Explosion);
            writer.Write(RollTime);
            writer.Write(RandomCircuitData);
            writer.Write(RandomGlyphData);
            writer.Write(ItemQuality2Id);
            writer.Write(MasterList.Count);
            MasterList.ForEach(m => m.Write(writer));
        }
    }
}
