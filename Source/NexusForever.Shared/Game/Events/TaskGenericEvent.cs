using System;
using System.Threading.Tasks;

namespace NexusForever.Shared.Game.Events
{
    public class TaskGenericEvent<T> : IEvent
    {
        private readonly Task<T> task;
        private readonly Action<T> callback;

        public TaskGenericEvent(Task<T> task, Action<T> callback)
        {
            this.task     = task;
            this.callback = callback;
        }

        public bool CanExecute()
        {
            return task.IsCompleted;
        }

        public void Execute()
        {
            callback.Invoke(task.Result);
        }
    }
}
