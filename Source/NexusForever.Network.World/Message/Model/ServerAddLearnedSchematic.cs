using NexusForever.Network.Message;
using NexusForever.Game.Static.Crafting;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAddLearnedSchematic)]
    public class ServerAddLearnedSchematic : IWritable
    {
        public TradeskillType TradeskillId { get; set; }
        public uint TradeskillSchematic2Id { get; set; }
        public Vector2 DiscoveryCoordinates { get; set; } // Coordinates on the discovery panel

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillId, 32u);
            writer.Write(TradeskillSchematic2Id);
            writer.Write(DiscoveryCoordinates.X);
            writer.Write(DiscoveryCoordinates.Y);
        }
    }
}
