namespace NexusForever.Script.Template
{
    public interface IOwnedScript<T> : IScript
    {
        /// <summary>
        /// Invoked when <see cref="IOwnedScript{T}"/> is loaded with <typeparamref name="T"/> owner.
        /// </summary>
        /// <remarks>
        /// This will be invoked after <see cref="IScript.OnLoad"/>.
        /// </remarks>
        void OnLoad(T owner)
        {
        }
    }
}
