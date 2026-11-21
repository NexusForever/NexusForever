using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Combat.CrowdControl;

namespace NexusForever.WorldServer.Network.Message.Handler.Combat
{
    public class ClientCCStateStunUpdateHandler : IMessageHandler<IWorldSession, ClientCCStateStunUpdate>
    {
        public void HandleMessage(IWorldSession session, ClientCCStateStunUpdate packet)
        {
        }
    }
}
