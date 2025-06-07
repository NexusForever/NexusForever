using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Unlike PublicEVentLocationUpdate, only allows for adding/removing to PublicEventObjectives, not to the PublicEvent
    [Message(GameMessageOpcode.ServerPublicEventMapRegionUpdate)]
    public class ServerPublicEventMapRegionUpdate : IWritable
    {
        public uint ObjectiveId { get; set; }
        public PublicEventOperationType Operation { get; set; }
        public MapRegion MapRegion { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ObjectiveId, 32u);
            writer.Write(Operation, 3u);
            MapRegion.Write(writer);
        }
    }
}
