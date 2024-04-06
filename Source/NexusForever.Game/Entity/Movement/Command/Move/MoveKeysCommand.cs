using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Move;
using NexusForever.Game.Entity.Movement.Key;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Move
{
    public class MoveKeysCommand : IMoveCommand
    {
        public EntityCommand Command => EntityCommand.SetMoveKeys;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => moveKeys?.IsFinalised ?? false;

        private MoveKeys moveKeys;

        /// <summary>
        /// Initialise command with the specified times and <see cref="Vector3"/> move key values.
        /// </summary>
        public void Initialise(IMovementManager movementManager, List<uint> times, List<Vector3> moves)
        {
            moveKeys = new MoveKeys();
            moveKeys.Initialise(movementManager, times, moves);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            // deliberately empty
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model   = new SetMoveKeysCommand
                {
                    Times  = moveKeys.Times,
                    Moves  = moveKeys.Values
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> move for the entity command.
        /// </summary>
        public Vector3 GetMove()
        {
            return moveKeys.GetInterpolated();
        }
    }
}
