using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestShareResultHandler : IMessageHandler<IWorldSession, ClientQuestShareResponse>
    {
        public void HandleMessage(IWorldSession session, ClientQuestShareResponse questShareResponse)
        {
            session.Player.QuestManager.QuestShareResult(questShareResponse.QuestId, questShareResponse.Accept);
        }
    }
}
