using Microsoft.Extensions.Logging;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueTime : IMatchingQueueTime
    {
        private const uint MaxSamples = 10u;

        public Static.Matching.MatchType MatchType { get; private set; } = Static.Matching.MatchType.None;

        /// <summary>
        /// Current average wait time for queue.
        /// </summary>
        public TimeSpan Average { get; private set; } = TimeSpan.FromMinutes(10d);

        private Queue<TimeSpan> samples = [];

        #region Dependency Injection

        private ILogger<IMatchingQueueTime> log;

        public MatchingQueueTime(
            ILogger<IMatchingQueueTime> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingQueueTime"/> with supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        public void Initialise(Static.Matching.MatchType matchType)
        {
            if (MatchType != Static.Matching.MatchType.None)
                throw new InvalidOperationException();

            MatchType = matchType;
        }

        /// <summary>
        /// Update average wait time with new sample.
        /// </summary>
        public void Update(TimeSpan timeSpan)
        {
            samples.Enqueue(timeSpan);
            if (samples.Count > MaxSamples)
                samples.Dequeue();

            Average = CalculateAverage();

            log.LogTrace($"Calculated new average {Average} for {MatchType} with new sample {timeSpan}.");
        }

        private TimeSpan CalculateAverage()
        {
            long totalTicks = 0;
            foreach (TimeSpan sample in samples)
                totalTicks += sample.Ticks;

            return TimeSpan.FromTicks(totalTicks / samples.Count);
        }
    }
}
