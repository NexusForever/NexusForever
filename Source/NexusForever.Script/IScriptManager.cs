using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Static;
using NexusForever.Script.Template.Collection;
using NexusForever.Shared;

namespace NexusForever.Script
{
    public interface IScriptManager : IUpdate
    {
        /// <summary>
        /// Id to be assigned to the next <see cref="IScriptInstanceInfo"/>.
        /// </summary>
        uint NextScriptId { get; }

        /// <summary>
        /// Id to be assigned to the next <see cref="IScriptCollection"/>.
        /// </summary>
        uint NextCollectionId { get; }

        /// <summary>
        /// Initialise <see cref="IScriptManager"/> and any associated resources.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Unload all <see cref="IScriptInstanceInfo"/> from supplied <see cref="IScriptCollection"/>.
        /// </summary>
        void Unload(IScriptCollection collection);

        /// <summary>
        /// Unload <see cref="IScriptAssemblyInfo"/> with supplied name.
        /// </summary>
        void Unload(string assemblyName);

        /// <summary>
        /// Reload <see cref="IScriptAssemblyInfo"/> with supplied name.
        /// </summary>
        void Reload(string assemblyName, ReloadType reloadType);

        /// <summary>
        /// Create a new <see cref="IScriptCollection"/> for supplied owner.
        /// </summary>
        IScriptCollection InitialiseOwnedCollection<T>(T owner);

        /// <summary>
        /// Initialise a new <see cref="IOwnedScriptCollection{T}"/> for supplied <typeparamref name="T"/> owner and id.
        /// </summary>
        void InitialiseOwnedScripts<T>(IScriptCollection collection, uint id);

        /// <summary>
        /// Initialise a new <see cref="IOwnedScriptCollection{T}"/> for supplied <typeparamref name="T"/> <see cref="IWorldEntity"/>.
        /// </summary>
        void InitialiseEntityScripts<T>(IScriptCollection collection, T entity) where T : IWorldEntity;

        string Information();
    }
}