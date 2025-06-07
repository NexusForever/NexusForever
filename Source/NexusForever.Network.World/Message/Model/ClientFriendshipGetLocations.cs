using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendshipGetLocations)]
    public class ClientFriendshipGetLocations : IReadable
    {
        public List<TargetPlayerIdentity> Identities { get; private set; } = []; // Identities of friends that are not account friends
        public List<ulong> AccountFriendIds { get; private set; } = []; // For friends that are account friends

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt(8);
            for (int i = 0; i < count; i++)
            {
                TargetPlayerIdentity identity = new TargetPlayerIdentity();
                identity.Read(reader);
                Identities.Add(identity);
            }

            count = reader.ReadUInt(8);
            for (int i = 0; i < count; i++)
            {
                AccountFriendIds.Add(reader.ReadULong());
            }
        }
    }
}
