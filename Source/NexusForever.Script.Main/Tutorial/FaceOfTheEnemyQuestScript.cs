using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// Exile/Dominion "The Face of the Enemy" combat tutorial (10518 / 10524).
    /// Queues the combat projector cinematic when the quest is accepted.
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
        private IQuest owner;

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public FaceOfTheEnemyQuestScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        public void OnLoad(IQuest owner)
        {
            this.owner = owner;
        }

        public void OnQuestStateChange(QuestState newState, QuestState oldState)
        {
            if (newState != QuestState.Accepted)
                return;

            // QuestAdd constructs with Accepted then assigns State=Accepted again (Accepted→Accepted).
            // Client accept from a mention is Mentioned→Accepted.
            if (oldState is not (QuestState.Accepted or QuestState.Mentioned or QuestState.Ignored or QuestState.Unknown))
                return;

            owner.Owner.CinematicManager.QueueCinematic(
                cinematicFactory.CreateCinematic<INoviceTutorialCombatProjector>());
        }
    }
}
