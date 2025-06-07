using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // If MatchType is a PvP type, vote was to surrender
    // If MatchType is a PvE type, vote was to disband
    [Message(GameMessageOpcode.ServerMatchingMatchVoteSurrenderFailed)]
    public class ServerMatchingMatchVoteSurrenderFailed : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
