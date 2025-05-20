using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendAccountCharacterZoneChanged)]
    public class ServerFriendAccountCharacterZoneChanged : IWritable
    {
        public class FriendCharacterZoneInfo : IWritable
        {
            public ulong AccountFriendId { get; set; }
            public TargetPlayerIdentity CharacterInNewZone { get; set; }
            public uint WorldZoneId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(AccountFriendId);
                CharacterInNewZone.Write(writer);
                writer.Write(WorldZoneId, 15u);
            }
        }

        public List<FriendCharacterZoneInfo> ZoneInfos { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ZoneInfos.Count);
            foreach (var zoneInfo in ZoneInfos)
            {
                zoneInfo.Write(writer);
            }
        }
    }
}
