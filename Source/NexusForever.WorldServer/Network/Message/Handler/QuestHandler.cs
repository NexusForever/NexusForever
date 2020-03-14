using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class QuestHandler
    {
        [MessageHandler(GameMessageOpcode.ClientQuestAbandon)]
        public static void HandleQuestAbandon(WorldSession session, ClientQuestAbandon questAbandon)
        {
            session.Player.QuestManager.QuestAbandon(questAbandon.QuestId);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestAccept)]
        public static void HandleQuestAccept(WorldSession session, ClientQuestAccept questAccept)
        {
            Item item = null;
            if (questAccept.ItemLocation.Location != (InventoryLocation)300)
                item = session.Player.Inventory.GetItem(questAccept.ItemLocation);

            session.Player.QuestManager.QuestAdd(questAccept.QuestId, item);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestComplete)]
        public static void HandleQuestComplete(WorldSession session, ClientQuestComplete questComplete)
        {
            session.Player.QuestManager.QuestComplete(questComplete.QuestId, questComplete.RewardSelection, questComplete.IsCommunique);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestSetIgnore)]
        public static void HandleQuestSetIgnore(WorldSession session, ClientQuestSetIgnore questSetIgnore)
        {
            session.Player.QuestManager.QuestIgnore(questSetIgnore.QuestId, questSetIgnore.Ignore);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestSetTracked)]
        public static void HandleQuestSetTracked(WorldSession session, ClientQuestSetTracked questSetTracked)
        {
            session.Player.QuestManager.QuestTrack(questSetTracked.QuestId, questSetTracked.Tracked);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestRetry)]
        public static void HandleQuestRetry(WorldSession session, ClientQuestRetry questRetry)
        {
            session.Player.QuestManager.QuestRetry(questRetry.QuestId);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestShareResult)]
        public static void HandleQuestShareResult(WorldSession session, ClientQuestShareResult questShareResult)
        {
            session.Player.QuestManager.QuestShareResult(questShareResult.QuestId, questShareResult.Result);
        }

        [MessageHandler(GameMessageOpcode.ClientQuestShare)]
        public static void HandleQuestShare(WorldSession session, ClientQuestShare questShare)
        {
            session.Player.QuestManager.QuestShare(questShare.QuestId);
        }
    }
}
