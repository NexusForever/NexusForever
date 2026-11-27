using NexusForever.Game.Quest;
using NexusForever.Network.Message;
using NexusForever.WorldServer.Network.Message.Handler.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestCompleteHandler : IMessageHandler<IWorldSession, ClientQuestComplete>
    {
        public void HandleMessage(IWorldSession session, ClientQuestComplete questComplete)
        {
            if (!MuseumInteraction.CanReport(session.Player, questComplete.QuestId, questComplete.IsCommunicatorMsg))
                throw new QuestException("The museum curator is not available to this player.");

            session.Player.QuestManager.QuestComplete(questComplete.QuestId, questComplete.RewardSelection, questComplete.IsCommunicatorMsg);
        }
    }
}
