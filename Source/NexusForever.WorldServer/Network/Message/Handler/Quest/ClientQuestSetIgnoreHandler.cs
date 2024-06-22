using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestSetIgnoreHandler : IMessageHandler<IWorldSession, ClientQuestSetIgnore>
    {
        public void HandleMessage(IWorldSession session, ClientQuestSetIgnore questSetIgnore)
        {
            session.Player.QuestManager.QuestIgnore(questSetIgnore.QuestId, questSetIgnore.Ignore);
        }
    }
}
