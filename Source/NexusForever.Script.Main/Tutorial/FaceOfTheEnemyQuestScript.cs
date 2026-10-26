using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// Exile/Dominion "The Face of the Enemy" combat tutorial (10518 / 10524) — NPE Part 2.
    /// Queues the combat projector cinematic when the quest is accepted.
    /// On achieve (all combat objectives done), force-completes and grants the housing quest
    /// (10525 / 10526). The player stays in the combat hologram and must activate the housing
    /// hologram projector (73741) to teleport into the plot.
    ///
    /// Exile objective flow (10518):
    /// 0. Kill 5 Battle Beasts
    /// 1. Detonate Easy sim-mine (73463)
    /// 2. Detonate Medium sim-mine (73667 → scripted Data=0)
    /// 3. Detonate Hard sim-mine (73668 → scripted Data=5968)
    /// 4. Kill 2 Dominion Turrets
    /// 5. Kill 3 Dominion Legionnaires (TG 14356 → 73492/73567)
    /// Dominion 10524 mirrors with Exile Elites (TG 14402 → 73473/73566).
    /// </summary>
    [ScriptFilterOwnerId(10518, 10524)]
    public class FaceOfTheEnemyQuestScript : IQuestScript, IOwnedScript<IQuest>
    {
        private const ushort ExileHousingQuestId    = 10525;
        private const ushort DominionHousingQuestId = 10526;

        private IQuest owner;
        private bool handedOff;

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;
        private readonly IGlobalQuestManager globalQuestManager;

        public FaceOfTheEnemyQuestScript(
            ICinematicFactory cinematicFactory,
            IGlobalQuestManager globalQuestManager)
        {
            this.cinematicFactory    = cinematicFactory;
            this.globalQuestManager  = globalQuestManager;
        }

        #endregion

        public void OnLoad(IQuest owner)
        {
            this.owner = owner;

            // Relog while Achieved (objectives done, hand-off never ran).
            if (owner.State == QuestState.Achieved && owner.Owner?.Map != null)
                CompleteCombatAndGrantHousing(owner.Owner);
        }

        public void OnQuestStateChange(QuestState newState, QuestState oldState)
        {
            if (newState == QuestState.Accepted)
            {
                // QuestAdd constructs with Accepted then assigns State=Accepted again (Accepted→Accepted).
                // Client accept from a mention is Mentioned→Accepted.
                if (oldState is not (QuestState.Accepted or QuestState.Mentioned or QuestState.Ignored or QuestState.Unknown))
                    return;

                if (owner.Owner != null)
                {
                    owner.Owner.CinematicManager.QueueCinematic(
                        cinematicFactory.CreateCinematic<INoviceTutorialCombatProjector>());
                }
                return;
            }

            if (newState == QuestState.Achieved)
                CompleteCombatAndGrantHousing(owner.Owner);
        }

        private void CompleteCombatAndGrantHousing(IPlayer player)
        {
            if (handedOff || player?.QuestManager == null)
                return;
            handedOff = true;

            try
            {
                // No quest receiver / communicator turn-in for this NPE quest.
                player.QuestManager.QuestForceComplete(owner.Id);

                ushort housingQuestId = player.Faction1 == Faction.Dominion
                    ? DominionHousingQuestId
                    : ExileHousingQuestId;

                if (player.QuestManager.GetQuestState(housingQuestId) != null)
                    return;

                IQuestInfo info = globalQuestManager.GetQuestInfo(housingQuestId);
                if (info == null)
                    return;

                // Obj0: interact with housing hologram projector 73741 (EnterZone 4965 on arrival).
                player.QuestManager.QuestAdd(info);
            }
            catch
            {
                handedOff = false;
                throw;
            }
        }
    }
}
