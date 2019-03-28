using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemUseDye, MessageDirection.Client)]
    public class ClientItemUseDye : IReadable
    {
        public InventoryLocation Location { get; private set; } // 9
        public uint BagIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Location = (InventoryLocation)reader.ReadUShort(9u);
            BagIndex = reader.ReadUInt();
        }
    }
}
