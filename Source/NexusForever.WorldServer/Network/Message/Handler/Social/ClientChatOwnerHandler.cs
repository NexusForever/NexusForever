using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatOwnerHandler : IMessageHandler<IWorldSession, ClientChatOwner>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatOwnerHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatOwner chatOwner)
        {
            ChatResult result = session.Player.ChatManager.CanPassOwner(chatOwner.Channel.ChatId, chatOwner.CharacterName);
            if (result != ChatResult.Ok)
            {
                globalChatManager.SendChatResult(session, chatOwner.Channel.Type, chatOwner.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.PassOwner(chatOwner.Channel.ChatId, chatOwner.CharacterName);
        }
    }
}
