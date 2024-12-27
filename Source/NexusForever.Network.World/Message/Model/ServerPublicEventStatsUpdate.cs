using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPublicEventStatsUpdate)]
    public class ServerPublicEventStatsUpdate : IWritable
    {
        public uint PublicEventId { get; set; }
        public List<PublicEventTeamStats> TeamStats { get; set; } = [];
        public List<PublicEventParticipantStats> ParticipantStats { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PublicEventId, 14);

            writer.Write(TeamStats.Count);
            foreach (PublicEventTeamStats stats in TeamStats)
                stats.Write(writer);

            writer.Write(ParticipantStats.Count);
            foreach (PublicEventParticipantStats stats in ParticipantStats)
                stats.Write(writer);
        }
    }
}
