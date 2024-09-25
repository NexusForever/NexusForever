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
        public List<uint> Locations { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ObjectiveId, 15);
            ObjectiveStatus.Write(writer);
            writer.Write(Busy);
            writer.Write(ElapsedTimeMs);
            writer.Write(NotificationMode);

            writer.Write((uint)Locations.Count);
            foreach (uint location in Locations)
                writer.Write(location);

            writer.Write(0); // some counter
        }
    }
}
