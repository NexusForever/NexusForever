using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Character
{
    public class CharacterListHandler : IMessageHandler<IWorldSession, ClientCharacterList>
    {
        private readonly ICharacterListManager characterListManager;
        private readonly ILoginQueueManager loginQueueManager;

        #region Dependency Injection

        public CharacterListHandler(
            ICharacterListManager characterListManager,
            ILoginQueueManager loginQueueManager)
        {
            this.characterListManager = characterListManager;
            this.loginQueueManager = loginQueueManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientCharacterList _)
        {
            // only handle session in queue once
            // TODO: might need to move this as HandleCharacterList is called multiple times
            if (!session.IsQueued.HasValue)
                loginQueueManager.OnNewSession(session);

            characterListManager.SendCharacterListPackets(session);
        }
    }
}
