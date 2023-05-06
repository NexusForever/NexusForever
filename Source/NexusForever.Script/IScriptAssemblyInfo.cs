using Microsoft.EntityFrameworkCore.Infrastructure;
using NexusForever.Script.Static;
using NexusForever.Script.Template.Collection;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script
{
    public interface IScriptAssemblyInfo
    {
        string Name { get; }
        string AssemblyPath { get; }
        string SourcePath { get; }

        /// <summary>
        /// Determines what type of reload is required for <see cref="IScriptAssemblyInfo"/>.
        /// </summary>
        ReloadType? Reload { get; set; }

        /// <summary>
        /// Initialise a new <see cref="IScriptAssemblyInfo"/> with supplied assembly and source locations.
        /// </summary>
        void Initialise(string name, string assemblyPath, string sourcePath);

        /// <summary>
        /// Load <see cref="IScriptAssemblyInfo"/> from assembly at supplied location.
        /// </summary>
        void LoadFromAssembly();

        /// <summary>
        /// Load <see cref="IScriptAssemblyInfo"/> from source at supplied location.
        /// </summary>
        void LoadFromSource();

        /// <summary>
        /// Unload <see cref="IScriptAssemblyInfo"/>.
        /// </summary>
        /// <remarks>
        /// This also will unload <see cref="IScriptInfo"/> contained in <see cref="IScriptAssemblyInfo"/> and unload <see cref="IScriptInstanceInfo"/> from <see cref="IScriptCollection"/>'s.
        /// </remarks>
        WeakReference Unload();

        /// <summary>
        /// Return a collection of <see cref="IScriptInfo"/>'s filtered with supplied <see cref="IScriptFilterSearch"/>.
        /// </summary>
        IEnumerable<IScriptInfo> Filter(IScriptFilterSearch search);

        /// <summary>
        /// Return a collection of <see cref="IScriptCollection"/>'s which contain <see cref="IScriptInstanceInfo"/>'s created from this <see cref="IScriptAssemblyInfo"/>.
        /// </summary>
        IEnumerable<IScriptCollection> GetScriptCollections();

        /// <summary>
        /// Build information about <see cref="IScriptAssemblyInfo"/>.
        /// </summary>
        void Information(IndentedStringBuilder sb);
    }
}