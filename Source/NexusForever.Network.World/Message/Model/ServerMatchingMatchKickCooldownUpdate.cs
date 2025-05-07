using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // If MatchQueueResult is GlobalKickCooldown, no wait time is sent
    // If MatchQueueResult is that personal or global kick cooldown is active, sends the WaitTime
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
