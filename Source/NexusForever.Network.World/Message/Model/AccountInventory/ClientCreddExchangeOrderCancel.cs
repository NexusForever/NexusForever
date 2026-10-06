using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.AccountInventory
{
    [Message(GameMessageOpcode.ClientCreddExchangeOrderCancel)]
    public class ClientCreddExchangeOrderCancel : IReadable
    {
        public Identity Unused { get; private set; } = new();
        public ulong CreddOrderId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unused.Read(reader);
            CreddOrderId = reader.ReadULong();
        }
    }
}
