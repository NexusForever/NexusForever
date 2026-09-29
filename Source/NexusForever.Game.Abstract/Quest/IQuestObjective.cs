using NexusForever.Database.Character;
using NexusForever.Shared;
using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Abstract.Quest
{
    public interface IQuestObjective : IUpdate, IDatabaseCharacter
    {
        IQuestInfo QuestInfo { get; }
        IQuestObjectiveInfo ObjectiveInfo { get; }
        byte Index { get; }
        uint Progress { get; set; }
        uint? Timer { get; set; }

        /// <summary>
        /// Return if the objective has been completed.
        /// </summary>
        bool IsComplete();

        /// <summary>
        /// Update object progress with supplied update.
        /// </summary>
        void ObjectiveUpdate(uint update);

        /// <summary>
        /// Set a checklist slot bit for <see cref="QuestObjectiveType.ActivateTargetGroupChecklist"/> objectives.
        /// </summary>
        /// <remarks>
        /// Progress is a bitfield sent to the client as completion flags. Each <paramref name="checklistIdx"/> sets one bit once.
        /// </remarks>
        void ObjectiveChecklistBit(byte checklistIdx);

        /// <summary>
        /// Complete this <see cref="IQuestObjective"/>.
        /// </summary>
        void Complete();
    }
}