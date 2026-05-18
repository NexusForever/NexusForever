using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestAbandonHandler : IMessageHandler<IWorldSession, ClientQuestAbandon>
    {
        public void HandleMessage(IWorldSession session, ClientQuestAbandon questAbandon)
        {
            session.Player.QuestManager.QuestAbandon(questAbandon.QuestId);
        }
    }
}
