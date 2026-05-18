using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Example
{
    [ScriptFilterIgnore]
    public class QuestScript : IQuestScript, IOwnedScript<IQuest>
    {
        private IQuest owner;

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IQuest owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
        }

        /// <summary>
        /// Invoked when <see cref="QuestState"/> is update for <see cref="IQuest"/>.
        /// </summary>
        void OnQuestStateChange(QuestState newState, QuestState oldState)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IQuestObjective"/> is updated for <see cref="IQuest"/>.
        /// </summary>
        void OnObjectiveUpdate(IQuestObjective objective)
        {
        }
    }
}
