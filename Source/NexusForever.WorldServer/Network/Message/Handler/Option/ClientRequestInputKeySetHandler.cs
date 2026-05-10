using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Option;

namespace NexusForever.WorldServer.Network.Message.Handler.Option
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
