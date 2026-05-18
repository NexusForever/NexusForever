using Microsoft.Extensions.Logging;

namespace NexusForever.Script.Watcher
{
    public abstract class Watcher : IWatcher
    {
        /// <summary>
        /// Determins if <see cref="IWatcher"/> is watching for file system events.
        /// </summary>
        public bool IsWatching => fileSystemWatcher?.EnableRaisingEvents ?? false;

        /// <summary>
        /// Invoked when file system event is raised.
        /// </summary>
        public event Action OnEvent;

        private FileSystemWatcher fileSystemWatcher;

        #region Dependency Injection

        private readonly ILogger log;

        public Watcher(
            ILogger log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IWatcher"/> with supplied path.
        /// </summary>
        public abstract void Initialise(string path);

        protected void Initialise(FileSystemWatcher watcher)
        {
            if (fileSystemWatcher != null)
                throw new InvalidOperationException();

            fileSystemWatcher = watcher;
        }

        /// <summary>
        /// Start listening to file system events.
        /// </summary>
        public void Start()
        {
            if (fileSystemWatcher == null)
                throw new InvalidOperationException();

            fileSystemWatcher.EnableRaisingEvents = true;
            log.LogTrace("Started listening for file events.");
        }

        /// <summary>
        /// Stop listening to file system events. 
        /// </summary>
        public void Stop()
        {
            if (fileSystemWatcher == null)
                throw new InvalidOperationException();

            fileSystemWatcher.EnableRaisingEvents = false;
            log.LogTrace("Stopped listening for file events.");
        }

        protected void RaiseEvent(object sender, FileSystemEventArgs e)
        {
            OnEvent.Invoke();
            log.LogTrace("Event raised for file event {ChangeType}.", e.ChangeType);
        }
    }
}
