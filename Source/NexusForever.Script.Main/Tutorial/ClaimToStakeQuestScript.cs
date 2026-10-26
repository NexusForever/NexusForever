using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// Exile/Dominion "A Claim to Stake" housing hologram (10525 / 10526) — NPE Part 4.
    /// Opening objective is EnterZone 4965 after activating housing projector 73741.
    /// Only credit EnterZone on load if the player is already inside the housing hologram
    /// (e.g. relog on the plot); do not skip the projector when still in the combat area.
    /// </summary>
    [ScriptFilterOwnerId(10525, 10526)]
    public class ClaimToStakeQuestScript : IQuestScript, IOwnedScript<IQuest>
    {
        private const uint HousingHologramZoneId = 4965u;

        private IQuest owner;

        public void OnLoad(IQuest owner)
        {
            this.owner = owner;
            CreditHousingEnterZoneIfPresent(owner.Owner);
        }

        public void OnQuestStateChange(QuestState newState, QuestState oldState)
        {
            if (newState != QuestState.Accepted)
                return;

            if (oldState is not (QuestState.Accepted or QuestState.Mentioned or QuestState.Ignored or QuestState.Unknown))
                return;

            CreditHousingEnterZoneIfPresent(owner.Owner);
        }

        private static void CreditHousingEnterZoneIfPresent(IPlayer player)
        {
            if (player?.QuestManager == null || player.Zone == null)
                return;

            // Housing plot subzone 5969 parents to hologram zone 4965.
            if (player.Zone.Id != HousingHologramZoneId && player.Zone.ParentZoneId != HousingHologramZoneId)
                return;

            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.EnterZone, HousingHologramZoneId, 1u);
        }
    }
}
