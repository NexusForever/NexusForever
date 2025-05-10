using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingPvpRatingUpdated)]
    public class ServerMatchingPvpRatingUpdated : IWritable
    {
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
