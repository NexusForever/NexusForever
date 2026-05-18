using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Loot
{
    [Message(GameMessageOpcode.ServerLootRoll)]
    public class ServerLootRoll : IWritable
    {
        public uint LootUnitId { get; set; }
        public Identity Roller { get; set; } = new Identity();
        public uint ItemId { get; set; } // Can be Item2Id, LootSpellId, VirtualItemId, AccountItemId
        public LootRollAction Action { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LootUnitId);
            Roller.Write(writer);
            writer.Write(ItemId, 18u);
            writer.Write(Action, 32u);
        }
    }
}
