using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatListHandler : IMessageHandler<IWorldSession, ClientChatList>
    {
        #region Dependency Injection

        private readonly IGlobalChatManager globalChatManager;

        public ClientChatListHandler(
            IGlobalChatManager globalChatManager)
        {
            this.globalChatManager = globalChatManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatList chatList)
        {
            ChatResult result = session.Player.ChatManager.CanListMembers(chatList.Channel.ChatId);
            if (result != ChatResult.Ok)
            {
                globalChatManager.SendChatResult(session, chatList.Channel.Type, chatList.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.ListMembers(chatList.Channel.ChatId);
        }
    }
}
