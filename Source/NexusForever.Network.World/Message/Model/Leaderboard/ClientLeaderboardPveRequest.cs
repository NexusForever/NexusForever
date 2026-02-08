using NexusForever.Game.Static.Leaderboard;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Leaderboard
{
    [Message(GameMessageOpcode.ClientLeaderboardPveRequest)]
    public class ClientLeaderboardPveRequest : IReadable
    {
        public LeaderboardType Type { get; private set; }
        public uint MatchingGameMapdId { get; private set; }
        public uint PrimeLevel { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<LeaderboardType>(4u);
            MatchingGameMapdId = reader.ReadUInt();
            PrimeLevel = reader.ReadUInt();
        }
    }
}
