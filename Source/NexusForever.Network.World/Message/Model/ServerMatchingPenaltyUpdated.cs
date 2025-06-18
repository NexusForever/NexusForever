using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingPenaltyUpdated)]
    public class ServerMatchingPenaltyUpdated : IWritable
    {
        public uint[] MatchingPenaltyTimesMS { get; set; } = new uint[16]; // These are the penalty times for each MatchType for the player
                                                                           // Array is ordered by Game.Static.Matching.MatchType
        public void Write(GamePacketWriter writer)
        {
            foreach (var penaltyTime in MatchingPenaltyTimesMS)
            {
                writer.Write(penaltyTime);
            }
        }
    }
}
