using System.Numerics;

namespace NexusForever.Game.Abstract.Entity.Movement.Command.Move
{
    public interface IMoveCommand : IEntityCommand
    {
        /// <summary>
        /// Returns the current <see cref="Vector3"/> move for the entity command.
        /// </summary>
        Vector3 GetMove();
    }
}
