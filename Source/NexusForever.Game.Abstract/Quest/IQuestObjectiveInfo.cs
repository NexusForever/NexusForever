using NexusForever.Game.Static.Quest;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Quest
{
    public interface IQuestObjectiveInfo
    {
        uint Id { get; }
        QuestObjectiveType Type { get; }

        QuestObjectiveEntry Entry { get; }

        /// <summary>
        /// Quest objective is sequential, previous quest objective must be complete.
        /// </summary>
        bool IsSequential();

        /// <summary>
        /// Quest objective is hidden until previous objective is complete.
        /// </summary>
        bool IsHidden();

        /// <summary>
        /// Quest objective is optional, doesn't need to be complete for quest to be achieved.
        /// </summary>
        bool IsOptional();

        bool HasUnknown0200();
    }
}