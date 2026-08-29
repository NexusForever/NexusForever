using NexusForever.Game.Static.Leaderboard;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Leaderboard
{
    [Message(GameMessageOpcode.ClientLeaderboardPvpRequest)]
    public class ClientLeaderboardPvpRequest : IReadable
    {
        public LeaderboardType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<LeaderboardType>(4u);
        }
    }
}
