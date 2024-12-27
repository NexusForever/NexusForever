using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Network.World.Entity;
using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Velocity;

namespace NexusForever.Game.Entity.Movement.Command.Velocity
{
    public class VelocityCommand : IVelocityCommand
    {
        public EntityCommand Command => EntityCommand.SetVelocity;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private Vector3 velocity;
        private bool blend;

        /// <summary>
        /// Initialise command with the specified <see cref="Vector3"/> velocity.
        /// </summary>
        public void Initialise(Vector3 velocity, bool blend)
        {
            this.velocity = velocity;
            this.blend    = blend;
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
                Model   = new SetVelocityCommand
                {
                    Velocity = velocity,
                    Blend    = blend
                }
            };
        }

        /// <summary>
        /// Get the current <see cref="Vector3"/> velocity value.
        /// </summary>
        public Vector3 GetVelocity()
        {
            return velocity;
        }
    }
}
