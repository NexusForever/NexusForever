using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientP2PTradingSetMoney)]
    public class ClientP2PTradingSetMoney : IReadable
    {
        public ulong Credits { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Credits = reader.ReadUInt();
        }
    }
}
