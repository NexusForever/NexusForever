using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
