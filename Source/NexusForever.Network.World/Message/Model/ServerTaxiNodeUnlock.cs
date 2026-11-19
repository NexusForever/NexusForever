using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Add a discovered TaxiNode to the client, the taxi map offers routes through it.
    /// </summary>
    [Message(GameMessageOpcode.ServerTaxiNodeUnlock)]
    public class ServerTaxiNodeUnlock : IWritable
    {
        public ushort TaxiNodeId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TaxiNodeId, 14u);
        }
    }
}
