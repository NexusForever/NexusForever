using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterOwnerId(3460)]
    public class TutorialMapScript : IMapScript, IOwnedScript<IBaseMap>
    {
        // Faction-specific "Navigating Nexus" starters (Quest2.QuestPlayerFactionEnum pairs).
        private const ushort ExileStarterQuestId     = 10527;
        private const ushort DominionStarterQuestId  = 10532;

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public TutorialMapScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IPlayer player)
                return;

            player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<INoviceTutorialOnEnter>());
            MentionStarterQuest(player);
        }

        private static void MentionStarterQuest(IPlayer player)
        {
            // Prefer mention over QuestAdd so the player accepts via the client offer UI.
            // Per-player only — no state stored on this shared map script.
            ushort questId = player.Faction1 == Faction.Dominion
                ? DominionStarterQuestId
                : ExileStarterQuestId;

            if (player.QuestManager.GetQuestState(questId) != null)
                return;

            player.QuestManager.QuestMention(questId);
        }
    }
}
