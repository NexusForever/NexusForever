using NexusForever.Network.Message;

namespace NexusForever.Network.World.Model.Loot
{
    // Usable when the player has been sent with ServerLootNotify (0x07F2)
    [Message(GameMessageOpcode.ClientLootVacuum)]
    public class ClientLootVacuum : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
