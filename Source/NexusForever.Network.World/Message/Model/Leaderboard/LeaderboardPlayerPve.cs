using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Leaderboard
{
    public class LeaderboardPlayerPve : IWritable
    {
        public ulong GuildId { get; set; }
        public Class Class { get; set; }
        public uint MatchingGameMapId { get; set; }
        public uint PrimeLevel { get; set; }
        public uint RewardedTier { get; set; }
        public uint CompletionTime { get; set; }
        public uint Rank { get; set; }
        public uint LastRank { get; set; }
        public string Name { get; set; }
        public List<TeamMember> TeamMembers { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GuildId);
            writer.Write(Class, 5u);
            writer.Write(MatchingGameMapId);
            writer.Write(PrimeLevel);
            writer.Write(RewardedTier);
            writer.Write(CompletionTime);
            writer.Write(Rank);
            writer.Write(LastRank);
            writer.WriteStringWide(Name);
            writer.Write(TeamMembers.Count);
            TeamMembers.ForEach(member => member.Write(writer));
        }
    }
}
