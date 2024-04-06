using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Velocity;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity.Movement.Command.Velocity
{
    public class VelocityDefaultsCommand : IVelocityCommand
    {
        public EntityCommand Command => EntityCommand.SetVelocityDefaults;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised { get; }

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
                Model   = new SetVelocityDefaultsCommand
                {
                }
            };
        }

        /// <summary>
        /// Return the current <see cref="Vector3"/> velocity value for the entity command.
        /// </summary>
        public Vector3 GetVelocity()
        {
            // TODO
            return Vector3.Zero;
        }
    }
}
