namespace NexusForever.Script.Template
{
    public interface IScript
    {
        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        void OnLoad()
        {
        }

        /// <summary>
        /// Invoked when <see cref="IScript"/> is unloaded.
        /// </summary>
        void OnUnload()
        {
        }
    }
}
