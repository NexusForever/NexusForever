using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestCompleteHandler : IMessageHandler<IWorldSession, ClientQuestComplete>
    {
        public void HandleMessage(IWorldSession session, ClientQuestComplete questComplete)
        {
            if (!Entity.DustrunnerBlackBox.CanComplete(session.Player, questComplete.QuestId, questComplete.IsCommunicatorMsg))
                return;

            session.Player.QuestManager.QuestComplete(questComplete.QuestId, questComplete.RewardSelection, questComplete.IsCommunicatorMsg);
        }
    }
}
