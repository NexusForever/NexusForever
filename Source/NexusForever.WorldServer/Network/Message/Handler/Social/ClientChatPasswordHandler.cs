using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatPasswordHandler : IMessageHandler<IWorldSession, ClientChatPassword>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatPasswordHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatPassword chatPassword)
        {
            ChatResult result = session.Player.ChatManager.CanSetPassword(chatPassword.Channel.ChatId, chatPassword.Password);
            if (result != ChatResult.Ok)
            {
                globalChatManager.SendChatResult(session, chatPassword.Channel.Type, chatPassword.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.SetPassword(chatPassword.Channel.ChatId, chatPassword.Password);
        }
    }
}
