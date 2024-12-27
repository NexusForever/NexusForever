using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestSetTrackedHandler : IMessageHandler<IWorldSession, ClientQuestSetTracked>
    {
        public void HandleMessage(IWorldSession session, ClientQuestSetTracked questSetTracked)
        {
            session.Player.QuestManager.QuestTrack(questSetTracked.QuestId, questSetTracked.Tracked);
        }
    }
}
