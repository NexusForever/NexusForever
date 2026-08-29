using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Only for adding a unit to a PublicEvent or PublicEventObjective. Ignores remove operations if specified.
    [Message(GameMessageOpcode.ServerPublicEventUnitAdd)]
    public class ServerPublicEventUnitAdd : IWritable
    {
        public class PublicEventOperation : IWritable
        {
            public uint ObjectId { get; set; } // PublicEventId or PublicEventObjectiveId depending on operation
            public PublicEventOperationType Operation { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ObjectId, 32u);
                writer.Write(Operation, 3u);
            }
        }

        public uint UnitId { get; set; }
        public List<PublicEventOperation> Operations { get; set; } = [];
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            Operations.ForEach(operation => operation.Write(writer));
        }
    }
}
