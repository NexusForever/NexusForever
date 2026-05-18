using System;

namespace NexusForever.Shared.Game.Events
{
    /// <summary>
    /// An <see cref="IEvent"/> that will be executed after predicate conditions are met.
    /// </summary>
    public class PredicateEvent : IEvent
    {
        private readonly Func<bool> predicate;
        private readonly Action callback;

        public PredicateEvent(Func<bool> predicate, Action callback)
        {
            this.predicate = predicate;
            this.callback  = callback;
        }

        /// <summary>
        /// Returns if <see cref="PredicateEvent"/> can be executed.
        /// </summary>
        public virtual bool CanExecute()
        {
            return predicate.Invoke();
        }

        /// <summary>
        /// Executes <see cref="PredicateEvent"/> action.
        /// </summary>
        public void Execute()
        {
            callback.Invoke();
        }
    }
}
