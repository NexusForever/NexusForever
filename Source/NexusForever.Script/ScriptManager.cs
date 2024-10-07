using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Configuration.Model;
using NexusForever.Script.Finder;
using NexusForever.Script.Static;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Collection;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared;

namespace NexusForever.Script
{
    public sealed class ScriptManager : Singleton<IScriptManager>, IScriptManager
    {
        /// <summary>
        /// Id to be assigned to the next <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        public uint NextScriptId => nextScriptId++;
        private uint nextScriptId = 1;

        /// <summary>
        /// Id to be assigned to the next <see cref="IScriptCollection"/>.
        /// </summary>
        public uint NextCollectionId => nextCollectionId++;
        private uint nextCollectionId = 1;

        private readonly Dictionary<string, IScriptAssemblyInfo> assemblies = new();

        #region Dependency Injection

        private readonly ILogger log;
        private readonly IOptions<ScriptConfig> config;

        private readonly IAssemblyFinder assemblyFinder;
        private readonly ISourceFinder sourceFinder;
        private readonly IFactory<IScriptAssemblyInfo> scriptAssemblyFactory;
        private readonly ICollectionFactory collectionFactory;

        public ScriptManager(
            ILogger<IScriptManager> log,
            IOptions<ScriptConfig> config,
            IAssemblyFinder assemblyFinder,
            ISourceFinder sourceFinder,
            IFactory<IScriptAssemblyInfo> scriptAssemblyFactory,
            ICollectionFactory collectionFactory)
        {
            this.log                   = log;
            this.config                = config;
            
            this.assemblyFinder        = assemblyFinder;
            this.sourceFinder          = sourceFinder;
            this.scriptAssemblyFactory = scriptAssemblyFactory;

            this.collectionFactory     = collectionFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IScriptManager"/> and any associated resources.
        /// </summary>
        public void Initialise()
        {
            log.LogInformation("Initialising scripts...");

            if (!config.Value.Enable)
            {
                log.LogInformation("Scripts are disabled!");
                return;
            }

            InitialiseScripts();
        }

        private void InitialiseScripts()
        {
            List<string> sourcePaths = config.Value.Dynamic.Enable ? sourceFinder.Find() : new List<string>();
            foreach (string assemblyPath in assemblyFinder.Find())
            {
                // attempt to find source path for this assembly
                string sourcePath = sourcePaths.SingleOrDefault(sourcePath => Path.GetFileName(sourcePath) == Path.GetFileNameWithoutExtension(assemblyPath));
                sourcePaths.Remove(sourcePath);

                IScriptAssemblyInfo assemblyInfo = CreateAssembly(Path.GetFileNameWithoutExtension(assemblyPath), assemblyPath, sourcePath);
                assemblyInfo.LoadFromAssembly();
            }

            // any remaining source paths have no assembly
            // these will be compiled into an assembly on load
            foreach (string sourcePath in sourcePaths)
            {
                IScriptAssemblyInfo assemblyInfo = CreateAssembly(Path.GetFileName(sourcePath), null, sourcePath);
                assemblyInfo.LoadFromSource();
            }
        }

        private IScriptAssemblyInfo CreateAssembly(string name, string assemblyPath, string sourcePath)
        {
            IScriptAssemblyInfo assemblyInfo = scriptAssemblyFactory.Resolve();
            assemblyInfo.Initialise(name, assemblyPath, sourcePath);
            assemblies.Add(assemblyInfo.Name, assemblyInfo);
            return assemblyInfo;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            foreach (IScriptAssemblyInfo assemblyInfo in assemblies.Values)
            {
                if (!assemblyInfo.Reload.HasValue)
                    continue;

                Reload(assemblyInfo, assemblyInfo.Reload.Value);
                assemblyInfo.Reload = null;
            }
        }

        /// <summary>
        /// Unload all <see cref="IScriptInstanceInfo"/> from supplied <see cref="IScriptCollection"/>.
        /// </summary>
        public void Unload(IScriptCollection collection)
        {
            log.LogTrace("Unloading script collection {Id}...", collection.Id);

            foreach (IScriptInstanceInfo instanceInfo in collection)
                instanceInfo.ScriptInfo.Unload(instanceInfo);
        }

        /// <summary>
        /// Unload <see cref="IScriptAssemblyInfo"/> with supplied name.
        /// </summary>
        public void Unload(string assemblyName)
        {
            if (assemblies.TryGetValue(assemblyName, out IScriptAssemblyInfo assembly))
                assembly.Unload();
        }

        private void Unload(IScriptAssemblyInfo assemblyInfo)
        {
            log.LogTrace("Unloading script assembly {Name}...", assemblyInfo.Name);

            WeakReference weakReference = assemblyInfo.Unload();
            for (uint i = 0; i < 25 && weakReference.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (i == 15)
                    log.LogWarning("Unloading script assembly {Name} is taking longer than expected...", assemblyInfo.Name);
            }

            if (weakReference.IsAlive)
                log.LogError("Failed to unload script assembly {Name}. One or more refernces to the assembly exists, investigate scripts!", assemblyInfo.Name);
            else
                log.LogTrace("Unloaded script assembly {Name} successfully.", assemblyInfo.Name);
        }

        /// <summary>
        /// Reload <see cref="IScriptAssemblyInfo"/> with supplied name.
        /// </summary>
        public void Reload(string assemblyName, ReloadType reloadType)
        {
            if (assemblies.TryGetValue(assemblyName, out IScriptAssemblyInfo assembly))
                Reload(assembly, reloadType);
        }

        private void Reload(IScriptAssemblyInfo assemblyInfo, ReloadType reloadType)
        {
            log.LogTrace("Starting reload {reloadType} for script assembly {Name}", assemblyInfo.Name, reloadType);

            List<IScriptCollection> collections = assemblyInfo
                .GetScriptCollections()
                .ToList();

            Unload(assemblyInfo);

            if (reloadType == ReloadType.Source)
                assemblyInfo.LoadFromSource();
            else
                assemblyInfo.LoadFromAssembly();

            foreach (IScriptCollection collection in collections)
                InitialiseScriptCollection(collection, collection.Search);
        }

        private IScriptCollection InitialiseOwnedCollection<T>(T owner)
        {
            IOwnedScriptCollection<T> collection = collectionFactory.CreateOwnedCollection<T>();
            collection.Initialise(owner);
            return collection;
        }

        private IEnumerable<IScriptInfo> FilterScripts(IScriptFilterSearch search)
        {
            foreach (IScriptAssemblyInfo assemblyInfo in assemblies.Values)
                foreach (IScriptInfo scriptInfo in assemblyInfo.Filter(search))
                    yield return scriptInfo;
        }

        private void InitialiseScriptCollection(IScriptCollection collection, IScriptFilterSearch search)
        {
            collection.Initialise(search);
            foreach (IScriptInfo scriptInfo in FilterScripts(collection.Search))
            {
                try
                {
                    scriptInfo.Load(collection);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "An exception occured during script load.");
                }
            }
        }

        /// <summary>
        /// Initialise a new <see cref="IOwnedScriptCollection{T}"/> for supplied <typeparamref name="T"/> owner and id.
        /// </summary>
        public IScriptCollection InitialiseOwnedScripts<T>(T owner, uint id)
        {
            IScriptCollection collection = InitialiseOwnedCollection(owner);
            InitialiseScriptCollection(collection, new ScriptFilterSearch()
                .FilterByScriptType<IOwnedScript<T>>()
                .FilterById(id));

            return collection;
        }

        /// <summary>
        /// Initialise a new <see cref="IOwnedScriptCollection{T}"/> for supplied <typeparamref name="T"/> <see cref="IWorldEntity"/>.
        /// </summary>
        public IScriptCollection InitialiseEntityScripts<T>(T entity) where T : IWorldEntity
        {
            IScriptCollection collection = InitialiseOwnedCollection(entity);
            InitialiseScriptCollection(collection, new ScriptFilterSearch()
                .FilterByScriptType<IOwnedScript<T>>()
                .FilterById(entity.EntityId)
                .FilterByCreatureId(entity.CreatureId)
                .FilterByActivePropId(entity.ActivePropId));
                //.FilterByTargetGroupId());

            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Information()
        {
            var sb = new IndentedStringBuilder();
            sb.AppendLine("Script Manager Information:");
            sb.AppendLine($"Assembly Count: {assemblies.Count}");

            foreach (IScriptAssemblyInfo assemblyInfo in assemblies.Values)
                assemblyInfo.Information(sb);

            return sb.ToString();
        }
    }
}
