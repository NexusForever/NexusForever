using Microsoft.EntityFrameworkCore.Infrastructure;
using NexusForever.Script.Template.Collection;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script
{
    public interface IScriptInfo
    {
        string Name { get; }
        public Type Type { get; }

        /// <summary>
        /// Initialise <see cref="IScriptInfo"/> with supplied <see cref="IScript"/> type.
        /// </summary>
        void Initialise(Type type);

        /// <summary>
        /// Determines if supplied <see cref="IScriptFilterSearch"/> can match with <see cref="IScriptInfo"/>.
        /// </summary>
        bool Match(IScriptFilterSearch search);

        /// <summary>
        /// Create a new <see cref="IScriptInstanceInfo"/> for <see cref="IScriptInfo"/> which is assigned to supplied <see cref="IScriptCollection"/>.
        /// </summary>
        void Load(IScriptCollection collection);

        // <summary>
        /// Unload a single existing <see cref="IScriptInstanceInfo"/> for <see cref="IScriptInfo"/>.
        /// </summary>
        void Unload(IScriptInstanceInfo instanceInfo);

        /// <summary>
        /// Unload all existing <see cref="IScriptInstanceInfo"/> for <see cref="IScriptInfo"/>.
        /// </summary>
        void Unload();

        /// <summary>
        /// Return a collection of <see cref="IScriptCollection"/>'s which contain <see cref="IScriptInstanceInfo"/>'s created from this <see cref="IScriptInfo"/>.
        /// </summary>
        IEnumerable<IScriptCollection> GetScriptCollections();

        /// <summary>
        /// Build information about <see cref="IScriptInfo"/>.
        /// </summary>
        void Information(IndentedStringBuilder sb);
    }
}