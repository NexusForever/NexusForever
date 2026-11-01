using System.Collections.Generic;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Every TaxiNode the player discovered, the taxi map only offers routes through these and nodes the client knows itself.
    /// </summary>
    [Message(GameMessageOpcode.ServerFlightPathUpdate)]
    public class ServerFlightPathUpdate : IWritable
    {
        public List<uint> TaxiNodeIds { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write((uint)TaxiNodeIds.Count);
            foreach (uint id in TaxiNodeIds)
                writer.Write(id);
        }
    }
}
