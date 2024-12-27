using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Move;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Move
{
    public class MoveDefaultsCommand : IMoveCommand
    {
        public EntityCommand Command => EntityCommand.SetMoveDefaults;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised { get; }

        private bool blend;

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            // deliberately empty
        }

        /// <summary>
        /// Initialise <see cref="MoveDefaultsCommand"/> with the specified blend.
        /// </summary>
        public void Initialise(bool blend)
        {
            this.blend = blend;
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model   = new SetMoveDefaultsCommand
                {
                    Blend = blend
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> move for the entity command.
        /// </summary>
        public Vector3 GetMove()
        {
            // TODO
            return Vector3.Zero;
        }
    }
}
