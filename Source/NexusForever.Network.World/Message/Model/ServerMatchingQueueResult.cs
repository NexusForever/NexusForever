using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingQueueResult)]
    public class ServerMatchingQueueResult : IWritable
    {
        public MatchingQueueResult Result { get; set; }
        public uint Unknown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 6u);
            writer.Write(Unknown, 4u);
        }
    }
}
