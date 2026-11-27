using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Quest;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.WorldServer.Network.Message.Handler.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestAcceptHandler : IMessageHandler<IWorldSession, ClientQuestAccept>
    {
        public void HandleMessage(IWorldSession session, ClientQuestAccept questAccept)
        {
            IItem item = null;
            if (questAccept.ItemLocation.Location != (InventoryLocation)300)
                item = session.Player.Inventory.GetItem(questAccept.ItemLocation);

            if (!MuseumInteraction.CanAccept(session.Player, questAccept.QuestId, item != null))
                throw new QuestException("The museum relic is not available to this player.");

            session.Player.QuestManager.QuestAdd(questAccept.QuestId, item);
        }
    }
}
