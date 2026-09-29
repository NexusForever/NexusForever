using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterOwnerId(3460)]
    public class TutorialMapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        // Faction-specific "Navigating Nexus" starters (Quest2.QuestPlayerFactionEnum pairs).
        private const ushort ExileStarterQuestId    = 10527;
        private const ushort DominionStarterQuestId = 10532;

        // "The Face of the Enemy" combat tutorial — granted after Navigating Nexus completes.
        private const ushort ExileCombatQuestId    = 10518;
        private const ushort DominionCombatQuestId = 10524;

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;
        private readonly IGlobalQuestManager globalQuestManager;

        public TutorialMapScript(
            ICinematicFactory cinematicFactory,
            IGlobalQuestManager globalQuestManager)
        {
            this.cinematicFactory   = cinematicFactory;
            this.globalQuestManager = globalQuestManager;
        }

        #endregion

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            // Opening cinematic only on first entry (starter not yet granted).
            if (player.QuestManager.GetQuestState(GetStarterQuestId(player)) == null)
                player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<INoviceTutorialOnEnter>());

            GrantStarterQuest(player);
            GrantCombatQuestIfReady(player);
        }

        private static ushort GetStarterQuestId(IPlayer player)
        {
            return player.Faction1 == Faction.Dominion
                ? DominionStarterQuestId
                : ExileStarterQuestId;
        }

        private static ushort GetCombatQuestId(IPlayer player)
        {
            return player.Faction1 == Faction.Dominion
                ? DominionCombatQuestId
                : ExileCombatQuestId;
        }

        private void GrantStarterQuest(IPlayer player)
        {
            // These starters have no CommunicatorMessages / quest givers in 3460, so QuestMention
            // never produces a visible client offer. Auto-add into the quest log instead.
            ushort questId = GetStarterQuestId(player);

            if (player.QuestManager.GetQuestState(questId) != null)
                return;

            IQuestInfo info = globalQuestManager.GetQuestInfo(questId);
            if (info == null)
                return;

            player.QuestManager.QuestAdd(info);
        }

        private void GrantCombatQuestIfReady(IPlayer player)
        {
            // After Navigating Nexus force-completes and teleports into the combat arena.
            if (player.QuestManager.GetQuestState(GetStarterQuestId(player)) != QuestState.Completed)
                return;

            ushort combatQuestId = GetCombatQuestId(player);
            if (player.QuestManager.GetQuestState(combatQuestId) != null)
                return;

            IQuestInfo info = globalQuestManager.GetQuestInfo(combatQuestId);
            if (info == null)
                return;

            // FaceOfTheEnemyQuestScript queues the combat projector cinematic on accept.
            player.QuestManager.QuestAdd(info);
        }
    }
}
