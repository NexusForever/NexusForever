using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCraftingCurrentCraft)]
    public class ServerCraftingCurrentCraft : IWritable
    {
        public uint TradeskillSchematic2Id { get; set; }
        public ulong CraftData { get; set; }
        public uint field_10_32bit { get; set; }
        public uint SchematicCount { get; set; }
        public uint AdditiveCount { get; set; }
        public uint field_1C_32bit { get; set; }
        public uint field_20_20bit { get; set; }
        public uint[] UnknownArray { get; set; } = new uint[4];
        public uint Item2Id { get; set; }
        public Vector2 UnknownVector1 { get; set; }
        public Vector2 UnknownVector2 { get; set; }
        public float field_48 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillSchematic2Id, 15);
            writer.Write(CraftData);
            writer.Write(field_10_32bit);
            writer.Write(SchematicCount);
            writer.Write(AdditiveCount);
            writer.Write(field_1C_32bit);
            writer.Write(field_20_20bit, 20);
            foreach (var id in UnknownArray)
            {
                writer.Write(id);
            }
            writer.Write(Item2Id, 18);
            writer.Write(UnknownVector1.X);
            writer.Write(UnknownVector1.Y);
            writer.Write(UnknownVector2.X);
            writer.Write(UnknownVector2.Y);
            writer.Write(field_48);
        }
    }
}
