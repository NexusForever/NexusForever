using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsAccountStatus)]
    public class ServerContactsAccountStatus : IWritable
    {
        public string AccountPublicStatus { get; set; }
        public string AccountNickname { get; set; }
        public ChatPresenceState Presence { get; set; }
        public bool BlockStrangerRequests { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(AccountPublicStatus);
            writer.WriteStringWide(AccountNickname);
            writer.Write(Presence, 3);
            writer.Write(BlockStrangerRequests);
        }
    }
}
