using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountEntitlements)]
    public class ServerAccountEntitlements : IWritable
    {
        public class AccountEntitlement : IWritable
        {
            public EntitlementType EntitlementId { get; set; }
            public uint Count { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(EntitlementId, 32u);
                writer.Write(Count);
            }
        }

        public List<AccountEntitlement> AccountEntitlements { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountEntitlements.Count);
            AccountEntitlements.ForEach(e => e.Write(writer));
        }
    }
}
