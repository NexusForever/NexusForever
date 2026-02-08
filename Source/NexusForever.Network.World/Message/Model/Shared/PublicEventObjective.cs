using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class PublicEventObjective : IWritable
    {
        public uint ObjectiveId { get; set; }
        public PublicEventObjectiveStatus ObjectiveStatus { get; set; }
        public bool Busy { get; set; }
        public uint ElapsedTimeMs { get; set; }
        public uint NotificationMode { get; set; }
        public List<uint> Locations { get; set; } = []; // array of worldLocation2Id
        public List<MapRegion> MapRegions { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ObjectiveId, 15);
            ObjectiveStatus.Write(writer);
            writer.Write(Busy);
            writer.Write(ElapsedTimeMs);
            writer.Write(NotificationMode);

            writer.Write(Locations.Count);
            Locations.ForEach(location => writer.Write(location));

            writer.Write(MapRegions.Count);
            MapRegions.ForEach(region => region.Write(writer));
        }
    }
}
