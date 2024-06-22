using NexusForever.Network.Message.Model;
using NexusForever.Network.Packet;

namespace NexusForever.Network.Message.Handler
{
    public class ClientEncryptedHandler : IMessageHandler<IGameSession, ClientEncrypted>
    {
        public void HandleMessage(IGameSession session, ClientEncrypted encrypted)
        {
            session.HandlePacket(new ClientGamePacket
            {
                Data        = encrypted.Data,
                IsEncrypted = true
            });
        }
    }
}
