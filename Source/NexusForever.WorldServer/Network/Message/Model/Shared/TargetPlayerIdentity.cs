using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class TargetPlayerIdentity : IReadable
    {
        public ushort RealmId { get; private set; }
        public ulong CharacterId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId     = reader.ReadUShort(14u);
            CharacterId = reader.ReadULong();
        }
    }
}
