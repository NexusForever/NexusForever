using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsSetPresence)]
    public class ServerContactsSetPresence : IWritable
    {
        public uint AccountId { get; set; }
        public ChatPresenceState Presence { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(Presence, 3);
        }
    }
}
