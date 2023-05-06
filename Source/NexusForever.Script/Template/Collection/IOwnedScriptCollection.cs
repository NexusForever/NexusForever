namespace NexusForever.Script.Template.Collection
{
    public interface IOwnedScriptCollection<T> : IScriptCollection
    {
        /// <summary>
        /// Initialise <see cref="IOwnedScriptCollection{T}"/> with supplied <typeparamref name="T"/> owner.
        /// </summary>
        void Initialise(T owner);
    }
}
