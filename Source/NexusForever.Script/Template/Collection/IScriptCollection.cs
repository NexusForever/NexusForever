using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Template.Collection
{
    public interface IScriptCollection : IEnumerable<IScriptInstanceInfo>
    {
        /// <summary>
        /// Unique identifier of <see cref="IScriptCollection"/>.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// <see cref="IScriptFilterSearch"/> used to populate the <see cref="IScriptCollection"/>.
        /// </summary>
        IScriptFilterSearch Search { get; }

        /// <summary>
        /// Initialise <see cref="IScriptCollection"/> with supplied <see cref="IScriptFilterSearch"/>.
        /// </summary>
        void Initialise(IScriptFilterSearch search);

        /// <summary>
        /// Load supplied <see cref="IScriptInstanceInfo"/> into <see cref="IScriptCollection"/>.
        /// </summary>
        void Load(IScriptInstanceInfo instanceInfo);

        /// <summary>
        /// Unload supplied <see cref="IScriptInstanceInfo"/> from <see cref="IScriptCollection"/>.
        /// </summary>
        void Unload(IScriptInstanceInfo instanceInfo);

        /// <summary>
        /// Invoke action on any scripts in <see cref="IScriptCollection"/> that are assignable to suppled <see cref="IScript"/> type.
        /// </summary>
        void Invoke<T>(Action<T> s);
    }
}
