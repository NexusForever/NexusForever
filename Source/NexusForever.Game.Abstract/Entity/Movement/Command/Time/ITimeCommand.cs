namespace NexusForever.Game.Abstract.Entity.Movement.Command.Time
{
    public interface ITimeCommand : IEntityCommand
    {
        /// <summary>
        /// Return the current time in milliseconds for the entity command.
        /// </summary>
        uint GetTime();
    }
}
