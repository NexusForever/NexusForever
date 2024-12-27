using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatLeaveHandler : IMessageHandler<IWorldSession, ClientChatLeave>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatLeaveHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatLeave chatLeave)
        {
            ChatResult result = session.Player.ChatManager.CanLeave(chatLeave.Channel.ChatId);
            if (result != ChatResult.Ok)
            {
                globalChatManager.SendChatResult(session, chatLeave.Channel.Type, chatLeave.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.Leave(chatLeave.Channel.ChatId, ChatChannelLeaveReason.Leave);
        }
    }
}
