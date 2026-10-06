using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientItemUseLootBag)]
    public class ClientItemUseLootBag : IReadable
    {
        public ItemLocation ItemLocation { get; private set; } = new ItemLocation();
        public ulong Guid { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemLocation.Read(reader);
            Guid = reader.ReadULong();
        }
    }
}
