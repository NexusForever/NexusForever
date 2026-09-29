using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Quest;
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

            player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<INoviceTutorialOnEnter>());
            GrantStarterQuest(player);
        }

        private void GrantStarterQuest(IPlayer player)
        {
            // These starters have no CommunicatorMessages / quest givers in 3460, so QuestMention
            // never produces a visible client offer. Auto-add into the quest log instead.
            ushort questId = player.Faction1 == Faction.Dominion
                ? DominionStarterQuestId
                : ExileStarterQuestId;

            if (player.QuestManager.GetQuestState(questId) != null)
                return;

            IQuestInfo info = globalQuestManager.GetQuestInfo(questId);
            if (info == null)
                return;

            player.QuestManager.QuestAdd(info);
        }
    }
}
