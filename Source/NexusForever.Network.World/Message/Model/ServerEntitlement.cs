using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntitlement)]
    public class ServerEntitlement : IWritable
    {
        public EntitlementType Entitlement { get; set; }
        public uint Count { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Entitlement, 14u);
            writer.Write(Count);
        }
    }
}
