using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountEntitlement)]
    public class ServerAccountEntitlement : IWritable
    {
        public EntitlementType EntitlementId { get; set; }
        public uint Count { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EntitlementId, 32u);
            writer.Write(Count);
        }
    }
}
