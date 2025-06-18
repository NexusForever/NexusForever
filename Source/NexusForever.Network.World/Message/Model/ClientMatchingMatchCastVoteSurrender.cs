using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Only sent if a surrender vote is active
    // If MatchType is a PvP type, is a vote to surrender
    // If MatchType is a PvE type, is a vote to disband
    [Message(GameMessageOpcode.ClientMatchingMatchCastVoteSurrender)]
    public class ClientMatchingMatchCastVoteSurrender : IReadable
    {
        public bool Vote { get; private set; } // true = yes, false = no

        public void Read(GamePacketReader reader)
        {
            Vote = reader.ReadBit();
        }
    }
}
