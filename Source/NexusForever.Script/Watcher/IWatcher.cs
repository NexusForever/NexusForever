namespace NexusForever.Script.Watcher
{
    public interface IWatcher
    {
        /// <summary>
        /// Determins if <see cref="IWatcher"/> is watching for file system events.
        /// </summary>
        bool IsWatching { get; }

        /// <summary>
        /// Invoked when file system event is raised.
        /// </summary>
        event Action OnEvent;

        /// <summary>
        /// Initialise <see cref="IWatcher"/> with supplied path.
        /// </summary>
        void Initialise(string path);

        /// <summary>
        /// Start listening to file system events.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop listening to file system events. 
        /// </summary>
        void Stop();
    }
}
