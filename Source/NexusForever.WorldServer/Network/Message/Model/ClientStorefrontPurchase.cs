using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontPurchase)]
    public class ClientStorefrontPurchase : IReadable
    {
        public StorefrontPurchase StorefrontPurchase { get; private set; } = new();

        public void Read(GamePacketReader reader)
        {
            StorefrontPurchase.Read(reader);
        }
    }
}
