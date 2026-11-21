using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Info
{
    [Message(GameMessageOpcode.ClientRealmInfoRequest)]
    public class ClientRealmInfoRequest : IReadable
    {
        public uint RealmId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId = reader.ReadUInt(14u);
        }
    }
}
