namespace NexusForever.Shared.Game.Events
{
    public interface IEvent
    {
        /// <summary>
        /// Returns if <see cref="IEvent"/> can be executed.
        /// </summary>
        bool CanExecute();

        /// <summary>
        /// Executes <see cref="IEvent"/> action.
        /// </summary>
        void Execute();
    }
}
