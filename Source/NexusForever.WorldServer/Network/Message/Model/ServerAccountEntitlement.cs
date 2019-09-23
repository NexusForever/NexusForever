using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountEntitlement)]
    public class ServerAccountEntitlement : IWritable
    {
        public EntitlementType Entitlement { get; set; }
        public uint Count { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Entitlement, 32u);
            writer.Write(Count);
        }
    }
}
