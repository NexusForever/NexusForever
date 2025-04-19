using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientP2PTradingRemoveItem)]
    public class ClientP2PTradingRemoveItem : IReadable
    {
        public ulong ItemGuid { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadUInt();
        }
    }
}
