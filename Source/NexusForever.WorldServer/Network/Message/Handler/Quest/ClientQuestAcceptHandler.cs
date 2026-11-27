using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientQuestAcceptHandler : IMessageHandler<IWorldSession, ClientQuestAccept>
    {
        public void HandleMessage(IWorldSession session, ClientQuestAccept questAccept)
        {
            bool fromItem = questAccept.ItemLocation.Location != (InventoryLocation)300;
            if (!Entity.DustrunnerBlackBox.CanAccept(session.Player, questAccept.QuestId, fromItem))
                return;

            IItem item = null;
            if (questAccept.ItemLocation.Location != (InventoryLocation)300)
                item = session.Player.Inventory.GetItem(questAccept.ItemLocation);

            session.Player.QuestManager.QuestAdd(questAccept.QuestId, item);
        }
    }
}
