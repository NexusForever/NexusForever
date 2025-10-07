using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Loot
{
    // Trigger BindOnPickup dialog when looting an item
    [Message(GameMessageOpcode.ServerLootBindOnPickup)]
    public class ServerLootBindOnPickup : IWritable
    {
        public uint Unused { get; set; }
        public uint LootUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused);
            writer.Write(LootUnitId);
        }
    }
}
