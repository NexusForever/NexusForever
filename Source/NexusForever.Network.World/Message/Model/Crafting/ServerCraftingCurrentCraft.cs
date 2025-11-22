using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ServerCraftingCurrentCraft)]
    public class ServerCraftingCurrentCraft : IWritable
    {
        public uint TradeskillSchematic2Id { get; set; }
        public CraftStats CraftStats { get; set; } = new CraftStats();
        public uint GlyphData { get; set; } // is of type RuneSlots in "Item / Inventory message" pull request
        public uint SchematicCount { get; set; }
        public uint AdditiveCount { get; set; }
        public uint Unused { get; set; } = 0;
        public uint[] Item2IdModifiers { get; set; } = new uint[5];
        public uint Item2Id { get; set; }
        public Vector2 DiscoveryCoordinates { get; set; }
        public Vector2 DiscoveryVectorMultiplier { get; set; }
        public float DiscoveryRadiusMultiplier { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillSchematic2Id, 15);
            CraftStats.Write(writer);
            writer.Write(GlyphData);
            writer.Write(SchematicCount);
            writer.Write(AdditiveCount);
            writer.Write(Unused);
            foreach (var id in Item2IdModifiers)
            {
                writer.Write(id);
            }
            writer.Write(Item2Id, 18);
            writer.Write(DiscoveryCoordinates.X);
            writer.Write(DiscoveryCoordinates.Y);
            writer.Write(DiscoveryVectorMultiplier.X);
            writer.Write(DiscoveryVectorMultiplier.Y);
            writer.Write(DiscoveryRadiusMultiplier);
        }
    }
}
