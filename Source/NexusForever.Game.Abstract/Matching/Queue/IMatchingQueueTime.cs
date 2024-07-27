
namespace NexusForever.Game.Matching.Queue
{
    public interface IMatchingQueueTime
    {
        Static.Matching.MatchType MatchType { get; }

        /// <summary>
        /// Current average wait time for queue.
        /// </summary>
        TimeSpan Average { get; }

        /// <summary>
        /// Initialise <see cref="IMatchingQueueTime"/> with supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        void Initialise(Static.Matching.MatchType matchType);

        /// <summary>
        /// Update average wait time with new sample.
        /// </summary>
        void Update(TimeSpan timeSpan);
    }
}
