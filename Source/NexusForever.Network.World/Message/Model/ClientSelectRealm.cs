using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientSelectRealm)]
    public class ClientSelectRealm : IReadable
    {
        public uint RealmId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId = reader.ReadUInt();
        }
    }
}
