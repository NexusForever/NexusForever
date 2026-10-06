using NexusForever.Game.Abstract.Entity.Synchronisation;

namespace NexusForever.Game.Entity.Synchronisation
{
    public class ActionSynchronisationTask : ISynchronisationTask
    {
        private TaskCompletionSource taskCompletionSource;
        private Action action;

        public Task Initialise(Action action)
        {
            if (taskCompletionSource != null)
                throw new InvalidOperationException("Synchronisation task has already been initialised.");

            this.action = action;

            taskCompletionSource = new TaskCompletionSource();
            return taskCompletionSource.Task;
        }

        public void Execute()
        {
            try
            {
                action();
                taskCompletionSource.SetResult();
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        }
    }
}
