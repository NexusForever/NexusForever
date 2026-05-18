using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Rotation
{
    public class RotationDefaultsCommand : IRotationCommand
    {
        public EntityCommand Command => EntityCommand.SetRotationDefaults;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private Vector3 rotation;
        private IPositionCommandGroup group;

        /// <summary>
        /// Initialise command with the specified <see cref="Vector3"/> rotation.
        /// </summary>
        public void Initialise(Vector3 rotation, IPositionCommandGroup group)
        {
            this.rotation = rotation;
            this.group    = group;
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
                Model   = new SetRotationDefaultsCommand
                {
                    Blend = false
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        public Vector3 GetRotation()
        {
            IPositionCommand command = group.Command;
            if (command.Command is EntityCommand.SetPositionSpline
                or EntityCommand.SetPositionPath
                or EntityCommand.SetPositionKeys)
                return command.GetRotation();

            return rotation;
        }
    }
}
