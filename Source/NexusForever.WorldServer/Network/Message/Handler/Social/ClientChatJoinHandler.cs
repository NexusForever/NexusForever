using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientChatJoinHandler : IMessageHandler<IWorldSession, ClientChatJoin>
    {
        public void HandleMessage(IWorldSession session, ClientChatJoin chatJoin)
        {
            ChatResult result = session.Player.ChatManager.CanJoin(chatJoin.Name, chatJoin.Password);
            if (result != ChatResult.Ok)
            {
                session.EnqueueMessageEncrypted(new ServerChatJoinResult
                {
                    Type   = chatJoin.Type,
                    Name   = chatJoin.Name,
                    Result = result
                });
                return;
            }

            session.Player.ChatManager.Join(chatJoin.Name, chatJoin.Password);
        }
    }
}
