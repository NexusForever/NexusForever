using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Move;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Move
{
    public class MoveCommand : IMoveCommand
    {
        public EntityCommand Command => EntityCommand.SetMove;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private Vector3 move;
        private bool blend;

        /// <summary>
        /// Initialise command with the specified <see cref="Vector3"/> and blend."/>
        /// </summary>
        public void Initialise(Vector3 move, bool blend)
        {
            this.move  = move;
            this.blend = blend;
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
                Model   = new SetMoveCommand
                {
                    Move  = move,
                    Blend = blend
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> move for the entity command.
        /// </summary>
        public Vector3 GetMove()
        {
            return move;
        }
    }
}
