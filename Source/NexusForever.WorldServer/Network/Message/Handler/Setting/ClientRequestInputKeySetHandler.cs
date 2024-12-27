using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Setting
{
    public class ClientRequestInputKeySetHandler : IMessageHandler<IWorldSession, ClientRequestInputKeySet>
    {
        public void HandleMessage(IWorldSession session, ClientRequestInputKeySet clientRequestInputKeySet)
        {
            if (clientRequestInputKeySet.CharacterId != 0ul)
                session.EnqueueMessageEncrypted(session.Player.KeybindingManager.Build());
            else
                session.EnqueueMessageEncrypted(session.Account.KeybindingManager.Build());
        }
    }
}
