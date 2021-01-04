using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildInviteResponse)]
    public class ClientGuildInviteResponse : IReadable
    {
        public bool Accepted { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Accepted = reader.ReadBit();
        }
    }
}
