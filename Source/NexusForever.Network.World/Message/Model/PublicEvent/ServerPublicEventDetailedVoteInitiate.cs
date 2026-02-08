using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventDetailedVoteInitiate)]
    public class ServerPublicEventDetailedVoteInitiate : IWritable
    {
        public class Tally : IWritable
        {
            public uint Choice { get; set; }
            public uint Count { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Choice);
                writer.Write(Count);
            }
        }

        public uint EventId { get; set; }
        public uint VoteId { get; set; }
        public PublicEventTeam TeamId { get; set; }
        public List<Tally> Tallies { get; set; } = [];
        public bool CanPlayerVote { get; set; }
        public uint ElapsedTimeMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventId, 14);
            writer.Write(VoteId, 14);
            writer.Write(TeamId, 14);
            writer.Write(Tallies.Count);

            foreach (Tally tally in Tallies)
                tally.Write(writer);

            writer.Write(CanPlayerVote);
            writer.Write(ElapsedTimeMs);
        }
    }
}
