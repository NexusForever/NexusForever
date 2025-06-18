using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingQueueLeaveAsGroup)]
    public class ClientMatchingQueueLeaveAsGroup : IReadable
    {
        public Game.Static.Matching.MatchType MatchType { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MatchType = reader.ReadEnum<Game.Static.Matching.MatchType>(5u);
        }
    }
}
