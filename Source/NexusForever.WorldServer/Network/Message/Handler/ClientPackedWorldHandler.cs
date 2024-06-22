using NexusForever.Network.Message;
using NexusForever.Network.Packet;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public class ClientPackedWorldHandler : IMessageHandler<IWorldSession, ClientPackedWorld>
    {
        public void HandleMessage(IWorldSession session, ClientPackedWorld packedWorld)
        {
            session.HandlePacket(new ClientGamePacket
            {
                Data        = packedWorld.Data,
                IsEncrypted = false
            });
        }
    }
}
