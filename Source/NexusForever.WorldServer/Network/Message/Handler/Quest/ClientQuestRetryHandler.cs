using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestRetryHandler : IMessageHandler<IWorldSession, ClientQuestRetry>
    {
        public void HandleMessage(IWorldSession session, ClientQuestRetry questRetry)
        {
            session.Player.QuestManager.QuestRetry(questRetry.QuestId);
        }
    }
}
