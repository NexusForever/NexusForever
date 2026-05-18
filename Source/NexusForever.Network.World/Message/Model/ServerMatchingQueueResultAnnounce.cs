using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingQueueResultAnnounce)]
    public class ServerMatchingQueueResultAnnounce : IWritable
    {
        public MatchingQueueResult Result { get; set; }
        public MatchingQueueStatus Status { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 6u);
            writer.Write(Status, 4u);
        }
    }
}
