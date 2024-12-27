namespace NexusForever.Game.Abstract.Entity.Movement.Command.Scale
{
    public interface IScaleCommand : IEntityCommand
    {
        /// <summary>
        /// Return the current scale for the entity command.
        /// </summary>
        float GetScale();
    }
}
