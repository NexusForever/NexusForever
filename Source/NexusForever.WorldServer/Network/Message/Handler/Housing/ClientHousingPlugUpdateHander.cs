using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingPlugUpdateHander : IMessageHandler<IWorldSession, ClientHousingPlugUpdate>
    {
        public void HandleMessage(IWorldSession session, ClientHousingPlugUpdate housingPlugUpdate)
        {
            // TODO
        }
    }
}
