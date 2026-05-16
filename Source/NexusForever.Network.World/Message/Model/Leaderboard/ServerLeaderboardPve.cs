using NexusForever.Game.Static.Leaderboard;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Leaderboard
{
    [Message(GameMessageOpcode.ServerLeaderboardPve)]
    public class ServerLeaderboardPve : IWritable
    {
        public LeaderboardType Type { get; set; }
        public uint MatchingGameMapId { get; set; }
        public uint PrimeLevel { get; set; }
        public ulong NextUpdateTime { get; set; } // Win32 FILETIME
        public List<LeaderboardPlayerPve> Players { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 4u);
            writer.Write(MatchingGameMapId);
            writer.Write(PrimeLevel);
            writer.Write(NextUpdateTime);
            writer.Write(Players.Count);
            Players.ForEach(player => player.Write(writer));
        }
    }
}
