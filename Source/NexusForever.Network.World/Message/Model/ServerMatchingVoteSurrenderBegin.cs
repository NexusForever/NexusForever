using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // If MatchType is a PvP type, starts a vote to surrender
    // If MatchType is a PvE type, starts a vote to disband
    [Message(GameMessageOpcode.ServerMatchingMatchVoteSurrenderBegin)]
    public class ServerMatchingMatchVoteSurrenderBegin : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
