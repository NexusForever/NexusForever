using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Can add or remove a unit to a PublicEvent or PublicEventOjective.
    [Message(GameMessageOpcode.ServerPublicEventUnitUpdate)]
    public class ServerPublicEventUnitUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public uint EventId { get; set; }
        public PublicEventOperationType Operation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(EventId, 32u);
            writer.Write(Operation, 3u);
        }
    }
}
