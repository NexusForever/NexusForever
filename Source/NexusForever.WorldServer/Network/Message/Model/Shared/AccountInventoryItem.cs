using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class AccountInventoryItem : IWritable
    {
        public ulong Id { get; set; }
        public uint ItemId { get; set; }
        public byte Unknown0 { get; set; } // 5
        public bool Unknown1 { get; set; }
        public TargetPlayerIdentity TargetPlayerIdentity { get; set; } = new TargetPlayerIdentity();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Id);
            writer.Write(ItemId);
            writer.Write(Unknown0, 5u);
            writer.Write(Unknown1);
            TargetPlayerIdentity.Write(writer);
        }
    }
}
