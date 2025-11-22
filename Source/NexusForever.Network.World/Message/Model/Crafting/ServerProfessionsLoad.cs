using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ServerProfessionsLoad)]
    public class ServerProfessionsLoad : IWritable
    {
        public class DiscoveredSchematic : IWritable
        {
            public uint TradeskillSchematic2Id { get; set; }
            public Vector2 Coordinates { get; set; } // Coordinates on the discovery panel

            public void Write(GamePacketWriter writer)
            {
                writer.Write(TradeskillSchematic2Id);
                writer.Write(Coordinates.X);
                writer.Write(Coordinates.Y);
            }
        }

        public List<TradeskillInfo> Tradeskills { get; set; } = [];
        public List<uint> LearnedSchematics { get; set; } = []; // TradeskillSchematic2Id
        public List<DiscoveredSchematic> DiscoveredSchematics { get; set; } = [];
        public List<uint> LearnedSchematicDiscoveredFlags { get; set; } = []; // which LearnedSchematics were discovered, bit order matches LearnedSchematics array
        public uint RelearnCooldown { get; set; } // Sent as an offset from the time now, to the finish time, in milliseconds.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Tradeskills.Count);
            foreach (var tradeskill in Tradeskills)
            {
                tradeskill.Write(writer);
            }

            writer.Write(LearnedSchematics.Count);
            foreach (var schematic in LearnedSchematics)
            {
                writer.Write(schematic);
            }

            writer.Write(DiscoveredSchematics.Count);
            foreach (var schematic in DiscoveredSchematics)
            {
                schematic.Write(writer);
            }

            writer.Write(LearnedSchematicDiscoveredFlags.Count);
            foreach (var id in LearnedSchematicDiscoveredFlags)
            {
                writer.Write(id);
            }

            writer.Write(RelearnCooldown);
        }
    }
}
