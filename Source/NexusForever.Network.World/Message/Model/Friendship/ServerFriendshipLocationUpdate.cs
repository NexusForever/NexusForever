using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipLocationUpdate)]
    public class ServerFriendshipLocationUpdate : IWritable
    {
        public class FriendLocation
        {
            public ulong FriendshipId { get; set; }
            public ushort WorldZoneId { get; set; }
        }

        public List<FriendLocation> FriendLocations { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendLocations.Count, 8);
            foreach (FriendLocation friendLocation in FriendLocations)
                writer.Write(friendLocation.FriendshipId);

            foreach (FriendLocation friendLocation in FriendLocations)
                writer.Write((uint)friendLocation.WorldZoneId, 32u);
        }
    }
}
