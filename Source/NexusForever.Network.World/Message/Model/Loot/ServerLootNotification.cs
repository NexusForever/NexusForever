using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Loot
{
    // Notifies the player when someone else loots an item.
    // Ignored if LooterUnitId is the player's UnitId
    [Message(GameMessageOpcode.ServerLootNotification)]
    public class ServerLootNotification : IWritable
    {
        public uint LootUnitId { get; set; }
        public uint ItemId { get; set; } // Can be Item2Id, LootSpellId, VirtualItemId, AccountItemId
        public uint Amount { get; set; }
        public uint LooterUnitId { get; set; }
        public LootItemType Type { get; set; }
        public ulong RandomCircuitData { get; set; }
        public uint RandomGlyphData { get; set; }
        public uint ItemQuality2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LootUnitId);
            writer.Write(ItemId);
            writer.Write(Amount);
            writer.Write(LooterUnitId);
            writer.Write(Type, 32u);
            writer.Write(RandomCircuitData);
            writer.Write(RandomGlyphData);
            writer.Write(ItemQuality2Id);
        }
    }
}
