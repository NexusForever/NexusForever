using NexusForever.Game.Static.Entity.Movement.Command.State;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.State
{
    public interface IStateCommand : IEntityCommand
    {
        /// <summary>
        /// Return the current <see cref="StateFlags"/> for the entity command.
        /// </summary>
        StateFlags GetState();
    }
}
