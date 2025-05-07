using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Triggers MatchVoteKickEnd lua event on client
    [Message(GameMessageOpcode.ServerMatchingMatchVoteKickCancelled)]
    public class ServerMatchingMatchVoteKickCancelled : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
