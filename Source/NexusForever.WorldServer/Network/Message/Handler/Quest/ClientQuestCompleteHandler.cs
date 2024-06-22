using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestCompleteHandler : IMessageHandler<IWorldSession, ClientQuestComplete>
    {
        public void HandleMessage(IWorldSession session, ClientQuestComplete questComplete)
        {
            session.Player.QuestManager.QuestComplete(questComplete.QuestId, questComplete.RewardSelection, questComplete.IsCommunique);
        }
    }
}
