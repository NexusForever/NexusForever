namespace NexusForever.Game.Static.Loot
{
    /// <summary>
    /// Condition types for conditional loot group evaluation.
    /// </summary>
    public enum LootConditionType
    {
        None                 = 0,
        IsClass              = 1,
        IsRace               = 2,
        IsLevel              = 3,
        IsLessThanLevel      = 4,
        IsMoreThanLevel      = 5,
        QuestIsComplete      = 6,
        QuestNotComplete     = 7,
        QuestObjectiveActive = 8
    }
}
