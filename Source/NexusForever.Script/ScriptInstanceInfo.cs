using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Collection;

namespace NexusForever.Script
{
    public class ScriptInstanceInfo : IScriptInstanceInfo
    {
        /// <summary>
        /// Unique identifier of <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        public uint Id { get; }

        public IScriptInfo ScriptInfo { get; private set; }
        public IScript Script { get; private set; }
        public IScriptCollection Collection { get; private set; }

        #region Dependency Injection

        private readonly ILogger log;
        private readonly IServiceProvider serviceProvider;

        public ScriptInstanceInfo(
            ILogger<IScriptInstanceInfo> log,
            IServiceProvider serviceProvider,
            IScriptManager scriptManager)
        {
            Id                   = scriptManager.NextScriptId;
            this.log             = log;
            this.serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        /// Load <see cref="IScriptInstanceInfo"/> based off supplied <see cref="IScriptInfo"/> for <see cref="IScriptCollection"/>.
        /// </summary>
        /// <remarks>
        /// This will create a new <see cref="IScript"/> instance and invoke the constructor with dependencies from dependency injection container.
        /// </remarks>
        public void Load(IScriptInfo scriptInfo, IScriptCollection collection)
        {
            if (Script != null)
                throw new InvalidOperationException();

            ScriptInfo = scriptInfo;
            Script     = (IScript)ActivatorUtilities.CreateInstance(serviceProvider, scriptInfo.Type);
            Collection = collection;

            log.LogTrace("Created new script instance {Name}({Id}).", ScriptInfo.Name, Id);

            collection.Load(this);
        }

        /// <summary>
        /// Unload <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        public void Unload()
        {
            Collection.Unload(this);
        }

        /// <summary>
        /// Build information about <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        public void Information(IndentedStringBuilder sb)
        {
            sb.IncrementIndent();
            sb.AppendLine($"Instance Id: {Id}");
            sb.AppendLine($"Instance Owner: {Collection.Id}");
            sb.DecrementIndent();
        }
    }
}
