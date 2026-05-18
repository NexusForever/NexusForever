namespace NexusForever.Game.Abstract.Entity.Movement.Command.Platform
{
    public interface IPlatformCommand : IEntityCommand
    {
        /// <summary>
        /// Returns the current platform unit id for the entity command.
        /// </summary>
        uint? GetPlatform();
    }
}
