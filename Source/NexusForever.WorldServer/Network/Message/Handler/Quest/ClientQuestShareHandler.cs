using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestShareHandler : IMessageHandler<IWorldSession, ClientQuestShare>
    {
        public void HandleMessage(IWorldSession session, ClientQuestShare questShare)
        {
            session.Player.QuestManager.QuestShare(questShare.QuestId);
        }
    }
}
