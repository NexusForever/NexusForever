using NexusForever.Game.Static.Entity.Movement.Command.Mode;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Mode
{
    public interface IModeCommand : IEntityCommand
    {
        /// <summary>
        /// Get the current <see cref="ModeType"/> value.
        /// </summary>
        ModeType GetMode();
    }
}
