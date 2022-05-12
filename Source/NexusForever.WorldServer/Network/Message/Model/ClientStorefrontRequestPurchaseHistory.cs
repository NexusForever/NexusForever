using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontRequestPurchaseHistory)]
    public class ClientStorefrontRequestPurchaseHistory : IReadable
    {
        public uint UnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
        }
    }
}
