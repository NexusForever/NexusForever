using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientContactsStatusChange)]
    public class ClientContactsStatusChange : IReadable
    {
        public ChatPresenceState Presence { get; set; }

        public void Read(GamePacketReader reader)
        {
            Presence = (ChatPresenceState)reader.ReadByte(3u);
        }
    }
}
