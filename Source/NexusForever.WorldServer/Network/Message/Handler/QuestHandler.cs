using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class QuestHandler
    {
        [MessageHandler(GameMessageOpcode.ClientQuestAbandon)]
        public static void HandleQuestAbandon(IWorldSession session, ClientQuestAbandon questAbandon)
        {
            session.Player.QuestManager.QuestAbandon(questAbandon.QuestId);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestAccept)]
        public static void HandleQuestAccept(IWorldSession session, ClientQuestAccept questAccept)
        {
            IItem item = null;
            if (questAccept.ItemLocation.Location != (InventoryLocation)300)
                item = session.Player.Inventory.GetItem(questAccept.ItemLocation);

            session.Player.QuestManager.QuestAdd(questAccept.QuestId, item);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestComplete)]
        public static void HandleQuestComplete(IWorldSession session, ClientQuestComplete questComplete)
        {
            session.Player.QuestManager.QuestComplete(questComplete.QuestId, questComplete.RewardSelection, questComplete.IsCommunique);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestSetIgnore)]
        public static void HandleQuestSetIgnore(IWorldSession session, ClientQuestSetIgnore questSetIgnore)
        {
            session.Player.QuestManager.QuestIgnore(questSetIgnore.QuestId, questSetIgnore.Ignore);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestSetTracked)]
        public static void HandleQuestSetTracked(IWorldSession session, ClientQuestSetTracked questSetTracked)
        {
            session.Player.QuestManager.QuestTrack(questSetTracked.QuestId, questSetTracked.Tracked);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestRetry)]
        public static void HandleQuestRetry(IWorldSession session, ClientQuestRetry questRetry)
        {
            session.Player.QuestManager.QuestRetry(questRetry.QuestId);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestShareResult)]
        public static void HandleQuestShareResult(IWorldSession session, ClientQuestShareResult questShareResult)
        {
            session.Player.QuestManager.QuestShareResult(questShareResult.QuestId, questShareResult.Result);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestShare)]
        public static void HandleQuestShare(IWorldSession session, ClientQuestShare questShare)
        {
            session.Player.QuestManager.QuestShare(questShare.QuestId);
        }
    }
}
