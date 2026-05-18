using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountEntitlements)]
    public class ServerAccountEntitlements : IWritable
    {
        public class AccountEntitlementInfo : IWritable
        {
            public EntitlementType Entitlement { get; set; }
            public uint Count { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Entitlement, 32u);
                writer.Write(Count);
            }
        }

        public List<AccountEntitlementInfo> Entitlements { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Entitlements.Count);
            Entitlements.ForEach(e => e.Write(writer));
        }
    }
}
