using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Collection;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared;

namespace NexusForever.Script
{
    public class ScriptInfo : IScriptInfo
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        private readonly Dictionary<uint, IScriptInstanceInfo> instances = new();

        #region Dependency Injection

        private readonly ILogger log;
        private readonly IFactory<IScriptInstanceInfo> instanceFactory;
        private readonly IScriptFilterParameters parameters;

        public ScriptInfo(
            ILogger<IScriptInfo> log,
            IFactory<IScriptInstanceInfo> instanceFactory,
            IScriptFilterParameters parameters)
        {
            this.log             = log;
            this.instanceFactory = instanceFactory;
            this.parameters      = parameters;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IScriptInfo"/> with supplied <see cref="IScript"/> type.
        /// </summary>
        public void Initialise(Type type)
        {
            Name = type.Name;
            Type = type;

            parameters.Initialise(type);

            log.LogTrace("Initialised script {Name}.", Name);
        }

        /// <summary>
        /// Determines if supplied <see cref="IScriptFilterSearch"/> can match with <see cref="IScriptInfo"/>.
        /// </summary>
        public bool Match(IScriptFilterSearch search)
        {
            IScriptFilterMatch match = new ScriptFilterMatch();
            return match.Match(search, parameters);
        }

        /// <summary>
        /// Create a new <see cref="IScriptInstanceInfo"/> for <see cref="IScriptInfo"/> which is assigned to supplied <see cref="IScriptCollection"/>.
        /// </summary>
        public void Load(IScriptCollection collection)
        {
            IScriptInstanceInfo instanceInfo = instanceFactory.Resolve();
            instanceInfo.Load(this, collection);

            instances.Add(instanceInfo.Id, instanceInfo);

            log.LogTrace("Added script instance {Name}({Id}).", Name, instanceInfo.Id);
        }

        /// <summary>
        /// Unload a single existing <see cref="IScriptInstanceInfo"/> for <see cref="IScriptInfo"/>.
        /// </summary>
        public void Unload(IScriptInstanceInfo instanceInfo)
        {
            instanceInfo.Unload();
            instances.Remove(instanceInfo.Id);

            log.LogTrace("Unloaded script instance {Name}({Id}).", Name, instanceInfo.Id);
        }

        /// <summary>
        /// Unload all existing <see cref="IScriptInstanceInfo"/> for <see cref="IScriptInfo"/>.
        /// </summary>
        public void Unload()
        {
            foreach (IScriptInstanceInfo instanceInfo in instances.Values)
                instanceInfo.Unload();

            instances.Clear();
        }

        /// <summary>
        /// Return a collection of <see cref="IScriptCollection"/>'s which contain <see cref="IScriptInstanceInfo"/>'s created from this <see cref="IScriptInfo"/>.
        /// </summary>
        public IEnumerable<IScriptCollection> GetScriptCollections()
        {
            foreach (IScriptInstanceInfo instanceInfo in instances.Values)
                yield return instanceInfo.Collection;
        }

        /// <summary>
        /// Build information about <see cref="IScriptInfo"/>.
        /// </summary>
        public void Information(IndentedStringBuilder sb)
        {
            sb.IncrementIndent();
            sb.AppendLine($"Script Name: {Name}");
            sb.AppendLine($"Script Type: {Type}");
            sb.AppendLine($"Script Instance Count: {instances.Count}");

            foreach (IScriptInstanceInfo instanceInfo in instances.Values)
                instanceInfo.Information(sb);

            sb.DecrementIndent();
        }
    }
}
