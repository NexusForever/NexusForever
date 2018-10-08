using System;
using System.Threading.Tasks;

namespace NexusForever.Shared.Game.Events
{
    public class TaskEvent : IEvent
    {
        private readonly Task task;
        private readonly Action callback;

        public TaskEvent(Task task, Action callback)
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
            callback.Invoke();
        }
    }
}
