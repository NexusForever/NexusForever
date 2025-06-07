using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendLocationUpdate)]
    public class ServerFriendLocationUpdate : IWritable
    {
        public class FriendLocation
        {
            public ulong FriendshipId { get; set; }
            public uint WorldZoneId { get; set; }
        }

        public List<FriendLocation> FriendLocations { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendLocations.Count);
            foreach(var friendLocation in FriendLocations)
            {
                writer.Write(friendLocation.FriendshipId);
            }
            foreach (var friendLocation in FriendLocations)
            {
                writer.Write(friendLocation.WorldZoneId);
            }
        }
    }
}
