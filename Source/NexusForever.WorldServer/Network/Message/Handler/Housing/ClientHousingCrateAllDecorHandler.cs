using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingCrateAllDecorHandler : IMessageHandler<IWorldSession, ClientHousingCrateAllDecor>
    {
        public void HandleMessage(IWorldSession session, ClientHousingCrateAllDecor housingCrateAllDecor)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.CrateAllDecor(housingCrateAllDecor.TargetResidence, session.Player);
        }
    }
}
