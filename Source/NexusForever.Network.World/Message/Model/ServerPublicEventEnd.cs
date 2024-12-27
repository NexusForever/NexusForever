using NexusForever.Game.Static.Event;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPublicEventEnd)]
    public class ServerPublicEventEnd : IWritable
    {
        public class PublicEventObjectiveStatus : IWritable
        {
            public uint ObjectiveId { get; set; }
            public PublicEventStatus Status { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ObjectiveId, 15);
                writer.Write(Status, 32);
            }
        }

        public uint PublicEventId { get; set; }
        public PublicEventRemoveReason Reason { get; set; }
        public uint ElapsedTimeMs { get; set; }
        public PublicEventStats Stats { get; set; }
        public List<PublicEventTeamStats> TeamStats { get; set; } = [];
        public List<PublicEventParticipantStats> ParticipantStats { get; set; } = [];
        public List<PublicEventObjectiveStatus> ObjectiveStatus { get; set; } = [];
        public PublicEventRewardTier RewardTier { get; set; }
        public PublicEventRewardType RewardType { get; set; }
        public uint[] RewardThreshold { get; set; } = new uint[3];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PublicEventId, 14);
            writer.Write(Reason, 32);
            writer.Write(ElapsedTimeMs);
            Stats.Write(writer);

            writer.Write(TeamStats.Count);
            foreach (PublicEventTeamStats stats in TeamStats)
                stats.Write(writer);

            writer.Write(ParticipantStats.Count);
            foreach (PublicEventParticipantStats stats in ParticipantStats)
                stats.Write(writer);

            writer.Write(ObjectiveStatus.Count);
            foreach (PublicEventObjectiveStatus status in ObjectiveStatus)
                status.Write(writer);

            writer.Write(RewardTier, 32u);
            writer.Write(RewardType, 32u);

            foreach (uint threshold in RewardThreshold)
                writer.Write(threshold);
        }
    }
}
