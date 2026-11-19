using System.Collections.Generic;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Sent from the taxi window (the legs of the route) or a shuttle prompt (the one route of the shuttle).
    /// </summary>
    /// <remarks>
    /// The values are TaxiRoute ids in order, not TaxiNode ids.
    /// </remarks>
    [Message(GameMessageOpcode.ClientTaxiFlight)]
    public class ClientTaxiFlight : IReadable
    {
        public List<uint> Route { get; } = [];

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (uint i = 0u; i < count && reader.BytesRemaining >= 4u; i++)
                Route.Add(reader.ReadUInt());
        }
    }
}
