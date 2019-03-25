using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class TargetPlayerIdentity : IReadable, IWritable
    {
        public ushort RealmId { get; set; }
        public ulong CharacterId { get; set; }

        public void Read(GamePacketReader reader)
        {
            RealmId     = reader.ReadUShort(14u);
            CharacterId = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(CharacterId);
        }
    }
}
