using System;
using System.Threading.Tasks;

namespace NexusForever.Shared.Game.Events
{
    /// <summary>
    /// An <see cref="IEvent"/> that will execute an <see cref="Action"/> after <see cref="Task"/> completion.
    /// </summary>
    public class TaskEvent : IEvent
    {
        private readonly Task task;
        private readonly Action callback;

        public TaskEvent(Task task, Action callback)
        {
            this.task     = task;
            this.callback = callback;
        }

        /// <summary>
        /// Returns if <see cref="TaskEvent"/> can be executed.
        /// </summary>
        public bool CanExecute()
        {
            return task.IsCompleted;
        }

        /// <summary>
        /// Executes <see cref="TaskEvent"/> action.
        /// </summary>
        public void Execute()
        {
            callback.Invoke();
        }
    }
}
