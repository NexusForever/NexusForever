using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientSelectRealm, MessageDirection.Client)]
    public class ClientSelectRealm : IReadable
    {
        public uint RealmId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId = reader.ReadUInt();
        }
    }
}
