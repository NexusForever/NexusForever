namespace NexusForever.Shared
{
    /// <summary>
    /// Interface for classes that support a graceful shutdown.
    /// </summary>
    public interface IShutdownAble
    {
        /// <summary>
        /// Called when shutting down.
        /// </summary>
        void Shutdown();
    }
}
