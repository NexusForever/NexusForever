using Microsoft.EntityFrameworkCore.Infrastructure;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Collection;

namespace NexusForever.Script
{
    public interface IScriptInstanceInfo
    {
        /// <summary>
        /// Unique identifier of <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        uint Id { get; }

        IScriptInfo ScriptInfo { get; }
        IScript Script { get; }
        IScriptCollection Collection { get; }

        /// <summary>
        /// Load <see cref="IScriptInstanceInfo"/> based off supplied <see cref="IScriptInfo"/> for <see cref="IScriptCollection"/>.
        /// </summary>
        /// <remarks>
        /// This will create a new <see cref="IScript"/> instance and invoke the constructor with dependencies from dependency injection container.
        /// </remarks>
        void Load(IScriptInfo scriptInfo, IScriptCollection collection);

        /// <summary>
        /// Unload <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        void Unload();

        /// <summary>
        /// Build information about <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        void Information(IndentedStringBuilder sb);
    }
}