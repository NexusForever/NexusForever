using NexusForever.Game.Abstract.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatWhisperHandler : IMessageHandler<IWorldSession, ClientChatWhisper>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatWhisperHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatWhisper whisper)
        {
            globalChatManager.HandleWhisperChat(session.Player, whisper);
        }
    }
}
