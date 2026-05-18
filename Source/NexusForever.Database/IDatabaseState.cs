namespace NexusForever.Database
{
    public interface IDatabaseState
    {
        /// <summary>
        /// Returns if entity is enqueued to be saved to the database.
        /// </summary>
        bool PendingCreate { get; }

        /// <summary>
        /// Returns if entity is enqueued to be deleted from the database.
        /// </summary>
        bool PendingDelete { get; }

        /// <summary>
        /// Enqueue entity to be deleted from the database.
        /// </summary>
        void EnqueueDelete(bool set);
    }
}
