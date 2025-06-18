using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // If MatchQueueResult is GlobalKickCooldown, no wait time is needs to be sent as the message does not use the field
    // If MatchQueueResult is that PersonalKickCooldown, uses the WaitTime to update client views
    [Message(GameMessageOpcode.ServerMatchingMatchKickCooldownUpdate)]
    public class ServerMatchingMatchKickCooldownUpdate : IWritable
    {
        public MatchingQueueResult Result { get; set; }
        public uint WaitTimeBeforeVoteMS { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 6);
            writer.Write(WaitTimeBeforeVoteMS);
        }
    }
}
