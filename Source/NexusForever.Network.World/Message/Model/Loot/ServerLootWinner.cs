using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Model.Loot
{
    [Message(GameMessageOpcode.ServerLootWinner)]
    public class ServerLootWinner : IWritable
    {
        public class LootRoll : IWritable
        {
            public Identity Identity { get; set; } = new Identity(); // If contains zero value for both properties, signals all players passed on loot
            public uint Value { get; set; } // -1 = assigned, 1 to 100 = greed rolls, 101 to 200 = need rolls
                                            // Client subtracts 100 from need rolls to get a 1 to 100 range

            public void Write(GamePacketWriter writer)
            {
                Identity.Write(writer);
                writer.Write(Value);
            }
        }

        public uint LootUnitId { get; set; }
        public LootRoll WinningRoll { get; set; } = new LootRoll();
        public uint ItemId { get; set; } // Can be Item2Id, LootSpellId, VirtualItemId, AccountItemId
        public List<LootRoll> OtherRolls { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LootUnitId);
            WinningRoll.Write(writer);
            writer.Write(ItemId, 18u);
            writer.Write(OtherRolls.Count);
            OtherRolls.ForEach(roll => roll.Write(writer));
        }
    }
}
