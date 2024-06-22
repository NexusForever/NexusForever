using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatKickHandler : IMessageHandler<IWorldSession, ClientChatKick>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatKickHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatKick chatKick)
        {
            ChatResult result = session.Player.ChatManager.CanKick(chatKick.Channel.ChatId, chatKick.CharacterName);
            if (result != ChatResult.Ok)
            {
                globalChatManager.SendChatResult(session, chatKick.Channel.Type, chatKick.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.Kick(chatKick.Channel.ChatId, chatKick.CharacterName);
        }
    }
}
