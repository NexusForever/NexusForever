using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatModeratorHandler : IMessageHandler<IWorldSession, ClientChatModerator>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatModeratorHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatModerator chatModerator)
        {
            ChatResult result = session.Player.ChatManager.CanMakeModerator(chatModerator.Channel.ChatId, chatModerator.CharacterName);
            if (result != ChatResult.Ok)
            {
                globalChatManager.SendChatResult(session, chatModerator.Channel.Type, chatModerator.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.MakeModerator(chatModerator.Channel.ChatId, chatModerator.CharacterName, chatModerator.Status);
        }
    }
}
