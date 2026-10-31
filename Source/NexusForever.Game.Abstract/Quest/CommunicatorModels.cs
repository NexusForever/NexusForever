using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Abstract.Quest
{
    /// <summary>
    /// What a communicator condition needs to know about a quest from Quest2.
    /// </summary>
    /// <param name="RepeatableAfterComplete">Quest2 flag 0x20 or a repeat period: state 11 also passes once completed.</param>
    public readonly record struct CommunicatorQuestInfo(byte ObjectiveCount, bool RepeatableAfterComplete, uint Flags);

    /// <summary>
    /// One quest/state pair of a communicator row. <paramref name="Quest"/> is null for a quest missing from Quest2.
    /// </summary>
    public readonly record struct CommunicatorCondition(ushort QuestId, uint TableState, CommunicatorQuestInfo? Quest);

    /// <summary>
    /// A quest event that arms a communicator row: state <paramref name="Value"/> (1-4) or objective index <paramref name="Value"/>.
    /// </summary>
    public readonly record struct CommunicatorTrigger(ushort QuestId, CommunicatorTriggerKind Kind, byte Value);

    /// <summary>
    /// A player's record of a quest as the client's condition getter (0x1405fbc40) sees it. <paramref name="Native"/> is a
    /// quest in the active list, Accepted or Achieved; only then do the objective bits count.
    /// </summary>
    public readonly record struct CommunicatorQuestSnapshot(QuestState Raw, bool Native, byte CompletedObjectives);

    /// <summary>
    /// A communicator row armed by <paramref name="Trigger"/>.
    /// </summary>
    public readonly record struct CommunicatorArm(CommunicatorTrigger Trigger, ICommunicatorMessage Message);
}
