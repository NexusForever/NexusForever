using NexusForever.Game;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Housing;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingRemodelHandler : IMessageHandler<IWorldSession, ClientHousingRemodel>
    {
        public void HandleMessage(IWorldSession session, ClientHousingRemodel housingRemodel)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.Remodel(housingRemodel.TargetResidence.ToGameIdentity(), session.Player, housingRemodel);
        }
    }
}
