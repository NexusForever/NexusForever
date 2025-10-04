using NexusForever.Game.Abstract.Entity.Synchronisation;

namespace NexusForever.Game.Entity.Synchronisation
{
    public class SynchronisationTask<T> : ISynchronisationTask
    {
        private TaskCompletionSource<T> taskCompletionSource;
        private Func<T> function;

        public Task<T> Initialise(Func<T> function)
        {
            if (taskCompletionSource != null)
                throw new InvalidOperationException("Synchronisation task has already been initialised.");

            this.function = function;

            taskCompletionSource = new TaskCompletionSource<T>();
            return taskCompletionSource.Task;
        }

        public void Execute()
        {
            try
            {
                taskCompletionSource.SetResult(function.Invoke());
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        }
    }
}
