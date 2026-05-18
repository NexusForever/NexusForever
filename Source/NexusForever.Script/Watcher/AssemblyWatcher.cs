using Microsoft.Extensions.Logging;

namespace NexusForever.Script.Watcher
{
    public class AssemblyWatcher : Watcher, IAssemblyWatcher
    {
        #region Dependency Injection

        public AssemblyWatcher(
            ILogger<IAssemblyWatcher> log)
            : base(log)
        {
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IAssemblyWatcher"/> with supplied path.
        /// </summary>
        public override void Initialise(string path)
        {
            var watcher = new FileSystemWatcher
            {
                Path                  = Path.GetDirectoryName(path),
                Filter                = Path.GetFileName(path),
                IncludeSubdirectories = true,
                NotifyFilter          = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size,
            };

            watcher.Changed += RaiseEvent;
            Initialise(watcher);
        }
    }
}
