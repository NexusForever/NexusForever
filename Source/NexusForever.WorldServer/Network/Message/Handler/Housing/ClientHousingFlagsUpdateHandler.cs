using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingFlagsUpdateHandler : IMessageHandler<IWorldSession, ClientHousingFlagsUpdate>
    {
        public void HandleMessage(IWorldSession session, ClientHousingFlagsUpdate flagsUpdate)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            residenceMap.UpdateResidenceFlags(flagsUpdate.TargetResidence, session.Player, flagsUpdate);
        }
    }
}
