namespace NexusForever.Game.Abstract.Entity.Movement.Command.Scale
{
    public interface IScaleCommandGroup : IEntityCommandGroup
    {
        /// <summary>
        /// Initialise <see cref="IScaleCommandGroup"/> with default command.
        /// </summary>
        void Initialise(IMovementManager movementManager);

        /// <summary>
        /// Get the current scale value.
        /// </summary>
        float GetScale();

        /// <summary>
        /// Set the scale to the supplied value.
        /// </summary>
        void SetScale(float scale);

        /// <summary>
        /// Set the scale to the interpolated value between supplied times and scales.
        /// </summary>
        void SetScaleKeys(List<uint> times, List<float> scales);
    }
}
