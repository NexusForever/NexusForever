using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.AccountInventory
{
    [Message(GameMessageOpcode.ClientAccountItemTake)]
    public class ClientAccountItemTake : IReadable
    {
        public ulong UserInventoryId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UserInventoryId = reader.ReadULong();
        }
    }
}
