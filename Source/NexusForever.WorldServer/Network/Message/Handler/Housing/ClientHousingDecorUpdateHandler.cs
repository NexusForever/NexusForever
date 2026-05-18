using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingDecorUpdateHandler : IMessageHandler<IWorldSession, ClientHousingDecorUpdate>
    {
        public void HandleMessage(IWorldSession session, ClientHousingDecorUpdate housingDecorUpdate)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.DecorUpdate(session.Player, housingDecorUpdate);
        }
    }
}
