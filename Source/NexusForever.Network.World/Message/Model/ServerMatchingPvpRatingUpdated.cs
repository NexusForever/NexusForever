using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingPvpRatingUpdated)]
    public class ServerMatchingPvpRatingUpdated : IWritable
    {
        public enum MatchingGameRatingType : byte
        {
            Arena2v2 = 0x0,
            Arena3v3 = 0x1,
            Arena5v5 = 0x2,
            RatedBattleground = 0x3,
            Warplot = 0x4,
        }

        public uint Rating;
        public uint Wins;
        public uint Losses;
        public uint Draws;
        MatchingGameRatingType Type;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Rating);
            writer.Write(Wins);
            writer.Write(Losses);
            writer.Write(Draws);
            writer.Write(Type, 3u);
        }
    }
}
