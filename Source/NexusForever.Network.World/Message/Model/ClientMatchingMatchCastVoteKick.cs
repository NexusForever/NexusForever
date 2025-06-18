using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    // Only sent if a kick vote is active
    [Message(GameMessageOpcode.ClientMatchingMatchCastVoteKick)]
    public class ClientMatchingMatchCastVoteKick : IReadable
    {
        public Identity MemberToKick { get; private set; } = new(); // The match member to kick
        public bool Vote { get; private set; } // true = yes, false = no

        public void Read(GamePacketReader reader)
        {
            MemberToKick.Read(reader);
            Vote = reader.ReadBit();
        }
    }
}
