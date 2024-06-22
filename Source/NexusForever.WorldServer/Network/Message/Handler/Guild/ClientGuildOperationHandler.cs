using NexusForever.Game.Abstract.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Guild
{
    public class ClientGuildOperationHandler : IMessageHandler<IWorldSession, ClientGuildOperation>
    {
        #region Dependency Injection

        private readonly IGlobalGuildManager globalGuildManager;

        public ClientGuildOperationHandler(
            IGlobalGuildManager globalGuildManager)
        {
            this.globalGuildManager = globalGuildManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGuildOperation clientGuildOperation)
        {
            globalGuildManager.HandleGuildOperation(session.Player, clientGuildOperation);
        }
    }
}
