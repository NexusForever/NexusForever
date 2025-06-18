using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendInviteMarkedSeen)]
    public class ClientFriendInviteMarkedSeen : IReadable
    {
        public List<ulong> InvitesSeen { get; private set; } = [];// Array of InviteIds

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt(16);
            for (int i = 0; i < count; i++)
            {
                InvitesSeen.Add(reader.ReadULong());
            }
        }
    }
}
