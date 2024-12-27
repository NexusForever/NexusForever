using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatMuteHandler : IMessageHandler<IWorldSession, ClientChatMute>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatMuteHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatMute chatMute)
        {
            ChatResult result = session.Player.ChatManager.CanMute(chatMute.Channel.ChatId, chatMute.CharacterName);
            if (result != ChatResult.Ok)
            {
                globalChatManager.SendChatResult(session, chatMute.Channel.Type, chatMute.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.Mute(chatMute.Channel.ChatId, chatMute.CharacterName, chatMute.Status);
        }
    }
}
