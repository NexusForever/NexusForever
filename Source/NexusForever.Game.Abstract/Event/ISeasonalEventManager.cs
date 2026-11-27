namespace NexusForever.Game.Abstract.Event
{
    public interface ISeasonalEventManager
    {
        /// <summary>
        /// Determine whether an unlinked quest or a quest in an active seasonal event may be started.
        /// </summary>
        bool IsQuestAvailable(uint questId);

        /// <summary>
        /// Determine whether the persisted entity and quest belong to the same event, independent of the current date.
        /// </summary>
        bool IsEntityInQuestEvent(uint entityId, uint questId);
    }
}
