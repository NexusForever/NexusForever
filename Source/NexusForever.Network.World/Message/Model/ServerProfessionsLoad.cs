using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model
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
        public List<uint> UnknownArray { get; set; } = [];
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

            writer.Write(UnknownArray.Count);
            foreach (var id in UnknownArray)
            {
                writer.Write(id);
            }

            writer.Write(RelearnCooldown);
        }
    }
}
