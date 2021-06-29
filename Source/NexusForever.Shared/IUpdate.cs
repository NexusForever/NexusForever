namespace NexusForever.Shared
{
    public interface IUpdate
    {
        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        void Update(double lastTick);
    }
}
