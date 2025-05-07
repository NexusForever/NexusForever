using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // If MatchQueueRsult is that personal or global surrender cooldown is active, sends the WaitTime
    [Message(GameMessageOpcode.ServerMatchingMatchOperationResult)]
    public class ServerMatchingMatchOperationResult : IWritable
    {
        public MatchingQueueResult Result { get; set; }
        public uint WaitTimeBeforeVoteMS { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result,6);
            writer.Write(WaitTimeBeforeVoteMS);
        }
    }
}
