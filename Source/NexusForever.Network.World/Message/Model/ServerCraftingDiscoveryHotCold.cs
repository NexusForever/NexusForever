using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCraftingDiscoveryHotCold)]
    public class ServerCraftingDiscoveryHotCold : IWritable
    {
        public bool Pass { get; set; }
        public uint TradeskillSchematic2IdCrafted { get; set; }
        public uint Item2IdCrafted { get; set; }
        public uint field_C_32bit { get; set; }
        public uint eHotOrCold_32bit { get; set; }
        public byte direction_4bit { get; set; }
        public uint earnedXp_32bit { get; set; }
        public List<uint> MaterialReturnedIds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Pass);
            writer.Write(TradeskillSchematic2IdCrafted, 15);
            writer.Write(Item2IdCrafted, 18);
            writer.Write(field_C_32bit);
            writer.Write((uint)eHotOrCold_32bit);
            writer.Write((uint)direction_4bit, 4);
            writer.Write(earnedXp_32bit);
            writer.Write(MaterialReturnedIds.Count);
            foreach (var id in MaterialReturnedIds)
            {
                writer.Write(id);
            }
        }
    }
}
