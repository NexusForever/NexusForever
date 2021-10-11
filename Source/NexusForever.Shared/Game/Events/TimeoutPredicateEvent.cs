using System;
using System.Threading;

namespace NexusForever.Shared.Game.Events
{
    /// <summary>
    /// An <see cref="IEvent"/> that will be executed after predicate conditions are met or timeout has elapsed.
    /// </summary>
    public class TimeoutPredicateEvent : PredicateEvent
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        public TimeoutPredicateEvent(TimeSpan timeSpan, Func<bool> predicate, Action callback)
            : base(predicate, callback)
        {
            cancellationTokenSource = new CancellationTokenSource(timeSpan);
        }

        public override bool CanExecute()
        {
            return base.CanExecute() || cancellationTokenSource.IsCancellationRequested;
        }
    }
}
