using NexusForever.Game.Static.Leaderboard;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Leaderboard
{
    [Message(GameMessageOpcode.ServerLeaderboardPvp)]
    public class ServerLeaderboardPvp : IWritable
    {
        public LeaderboardType Type { get; set; }
        public ulong NextUpdateTime { get; set; } // Win32 FILETIME
        public List<LeaderboardTeamPvp> Players { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 4u);
            writer.Write(NextUpdateTime);
            writer.Write(Players.Count);
            Players.ForEach(player => player.Write(writer));
        }
    }
}
