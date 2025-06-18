using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingQueueRandomParty)]
    public class ClientMatchingQueueRandomParty
    {
        public Game.Static.Matching.MatchType MatchType { get; private set; }
        public MatchingQueueFlags Flags { get; private set; }
        public Role Roles { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MatchType = reader.ReadEnum<Game.Static.Matching.MatchType>(5u);
            Flags = reader.ReadEnum<MatchingQueueFlags>(32u);
            Roles = reader.ReadEnum<Role>(32u);
        }
    }
}
