using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Seasonal.ShadeEve
{
    [ScriptFilterOwnerId(9760, 9761)]
    public sealed class ShadeEveIntroductionQuestScript : IQuestScript, IOwnedScript<IQuest>
    {
        private IQuest quest;

        public void OnLoad(IQuest owner)
        {
            quest = owner;
        }

        public void Update(double lastTick)
        {
            // The first quest tick runs after the journal has registered the accepted quest.
            if (quest is { State: QuestState.Accepted, PendingDelete: false } && quest.Info.Objectives.IsEmpty)
                quest.State = QuestState.Achieved;
        }
    }
}
