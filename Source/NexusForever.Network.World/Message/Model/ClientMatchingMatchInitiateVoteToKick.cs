using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingMatchInitiateVoteToKick)]
    public class ClientMatchingMatchInitiateVoteToKick : IReadable
    {
        public TargetPlayerIdentity MemberToKick { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MemberToKick = new TargetPlayerIdentity();
            MemberToKick.Read(reader);
        }
    }
}
