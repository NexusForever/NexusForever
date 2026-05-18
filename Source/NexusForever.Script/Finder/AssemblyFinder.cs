using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.Script.Configuration.Model;

namespace NexusForever.Script.Finder
{
    public class AssemblyFinder : IAssemblyFinder
    {
        #region Dependency Injection

        private readonly ILogger log;
        private readonly IOptions<ScriptConfig> config;

        public AssemblyFinder(
            ILogger<IAssemblyFinder> log,
            IOptions<ScriptConfig> config)
        {
            this.log    = log;
            this.config = config;
        }

        #endregion

        /// <summary>
        /// Return a collection of assembly source files.
        /// </summary>
        public List<string> Find()
        {
            // search for assemblies in explicit directory
            if (!string.IsNullOrWhiteSpace(config.Value.Directory)
                && Find(config.Value.Directory, out List<string> list))
                return list;

            // search for assemblies in default directory
            if (Find(".", out list))
                return list;

            return new List<string>();
        }

        private bool Find(string path, out List<string> list)
        {
            log.LogTrace("Searching for script assembly directories in {path}", path);

            list = Directory.GetFiles(path, "NexusForever.Script.*.dll")
                .ToList();
            return list.Count > 0;
        }
    }
}
