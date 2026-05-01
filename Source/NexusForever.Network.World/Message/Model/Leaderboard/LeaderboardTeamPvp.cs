using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Leaderboard
{
    public class LeaderboardTeamPvp : IWritable
    {
        public ulong GuildId { get; set; }
        public Class Class { get; set; } // value is 23 when leaderboard type is Arena3v3
        public uint Rating { get; set; }
        public uint Rank { get; set; }
        public uint LastRank { get; set; }
        public string Name { get; set; } // can be team name or player name, depending on the leaderboard type
        public List<TeamMember> TeamMembers { get; set; } = []; // Battleground rankings are for individuals only, there are no team members

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GuildId);
            writer.Write(Class, 5u);
            writer.Write(Rating);
            writer.Write(Rank);
            writer.Write(LastRank);
            writer.WriteStringWide(Name);
            writer.Write(TeamMembers.Count);
            TeamMembers.ForEach(member => member.Write(writer));
        }
    }
}
