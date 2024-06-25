using NexusForever.Network.Message.Model;
using NexusForever.Network.Packet;
using NexusForever.Network.Session;

namespace NexusForever.Network.Message.Handler
{
    public class ClientPackedHandler : IMessageHandler<IGameSession, ClientPacked>
    {
        public void HandleMessage(IGameSession session, ClientPacked packed)
        {
            session.HandlePacket(new ClientGamePacket
            {
                Data        = packed.Data,
                IsEncrypted = true
            });
        }
    }
}
