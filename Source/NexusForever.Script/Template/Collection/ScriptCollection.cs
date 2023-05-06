using System.Collections;
using Microsoft.Extensions.Logging;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Template.Collection
{
    public class ScriptCollection : IScriptCollection
    {
        /// <summary>
        /// Unique identifier of <see cref="IScriptCollection"/>.
        /// </summary>
        public uint Id { get; private set; }

        /// <summary>
        /// <see cref="IScriptFilterSearch"/> used to populate the <see cref="IScriptCollection"/>.
        /// </summary>
        public IScriptFilterSearch Search { get; private set; }

        private readonly Dictionary<uint, IScriptInstanceInfo> scripts = new();

        #region Dependency Injection

        private readonly ILogger<IScriptCollection> log;

        public ScriptCollection(
            ILogger<IScriptCollection> log,
            IScriptManager scriptManager)
        {
            Id = scriptManager.NextCollectionId;
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IScriptCollection"/> with supplied <see cref="IScriptFilterSearch"/>.
        /// </summary>
        public void Initialise(IScriptFilterSearch search)
        {
            Search = search;
        }

        /// <summary>
        /// Load supplied <see cref="IScriptInstanceInfo"/> into <see cref="IScriptCollection"/>.
        /// </summary>
        public virtual void Load(IScriptInstanceInfo instanceInfo)
        {
            scripts.Add(instanceInfo.Id, instanceInfo);
            log.LogTrace("Added script instance {instanceInfoId} to collection {Id}.", instanceInfo.Id, Id);

            try
            {
                instanceInfo.Script.OnLoad();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An excpetion occured during OnLoad for script {Id}, {Name}!", instanceInfo.Id, instanceInfo.ScriptInfo.Name);
            }
        }

        /// <summary>
        /// Unload supplied <see cref="IScriptInstanceInfo"/> from <see cref="IScriptCollection"/>.
        /// </summary>
        public void Unload(IScriptInstanceInfo instanceInfo)
        {
            try
            {
                instanceInfo.Script.OnUnload();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An excpetion occured during OnUnload for script {Id}, {Name}!", instanceInfo.Id, instanceInfo.ScriptInfo.Name);
            }

            scripts.Remove(instanceInfo.Id);
            log.LogTrace("Removed script instance {instanceInfoId} from collection {Id}.", instanceInfo.Id, Id);
        }

        /// <summary>
        /// Invoke action on any scripts in <see cref="IScriptCollection"/> that are assignable to suppled <see cref="IScript"/> type.
        /// </summary>
        public void Invoke<T>(Action<T> p)
        {
            foreach (IScriptInstanceInfo instanceInfo in scripts.Values)
                if (instanceInfo.ScriptInfo.Type.IsAssignableTo(typeof(T)))
                    p.Invoke((T)instanceInfo.Script);
        }

        public IEnumerator<IScriptInstanceInfo> GetEnumerator()
        {
            return scripts.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
