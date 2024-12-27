using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.Script.Configuration.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NexusForever.Script.Finder
{
    public class SourceFinder : ISourceFinder
    {
        #region Dependency Injection

        private readonly ILogger log;
        private readonly IOptions<ScriptConfig> config;

        public SourceFinder(
            ILogger<ISourceFinder> log,
            IOptions<ScriptConfig> config)
        {
            this.log    = log;
            this.config = config;
        }

        #endregion

        /// <summary>
        /// Return a collection of paths containing script sources.
        /// </summary>
        public List<string> Find()
        {
            // search for scripts in explicit directory
            if (!string.IsNullOrWhiteSpace(config.Value.Dynamic.Directory)
                && Find(config.Value.Dynamic.Directory, out List<string> list))
                return list;

            // running from build directory, find root source directory
            if (Find(Path.Combine("..", "..", "..", ".."), out list))
                return list;

            return new List<string>();
        }

        private bool Find(string path, out List<string> list)
        {
            log.LogTrace("Searching for script source directories in {path}", path);

            list = Directory.EnumerateDirectories(path, "NexusForever.Script.*")
                // why is this needed...
                .Where(d => !d.EndsWith("NexusForever.Script"))
                .ToList();
            return list.Count > 0;
        }
    }
}

