using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupJoinRequest)]
    public class ClientGroupJoinRequest : IReadable
    {
        public string GroupMemberName { get; private set; } // Name of character in group to join
        public string RealmName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupMemberName = reader.ReadWideString();
            RealmName = reader.ReadWideString();
        }
    }
}
