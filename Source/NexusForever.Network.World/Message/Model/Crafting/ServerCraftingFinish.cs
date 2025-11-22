using NexusForever.Network.Message;
using NexusForever.Game.Static.Crafting;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ServerCraftingFinish)]
    public class ServerCraftingFinish : IWritable
    {
        public bool Pass { get; set; }
        public uint TradeskillSchematic2IdCrafted { get; set; }
        public uint Item2IdCrafted { get; set; }
        public uint Unused { get; set; }
        public CraftingDiscovery HotOrCold { get; set; }
        public CraftingDirection Direction { get; set; }
        public uint EarnedXp { get; set; }
        public List<uint> MaterialReturnedIds { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Pass);
            writer.Write(TradeskillSchematic2IdCrafted, 15);
            writer.Write(Item2IdCrafted, 18);
            writer.Write(Unused);
            writer.Write(HotOrCold, 32u);
            writer.Write(Direction, 4);
            writer.Write(EarnedXp);
            writer.Write(MaterialReturnedIds.Count);
            foreach (var id in MaterialReturnedIds)
            {
                writer.Write(id);
            }
        }
    }
}
