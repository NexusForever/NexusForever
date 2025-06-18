using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    // Triggers MatchVoteKickBegin lua event on client
    [Message(GameMessageOpcode.ServerMatchingMatchVoteKickBegin)]
    public class ServerMatchingMatchVoteKickBegin : IWritable
    {
        public Identity Initiator { get; set; } = new();
        public Identity MemberToKick { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            Initiator.Write(writer);
            MemberToKick.Write(writer);
        }
    }
}
