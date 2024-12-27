using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Rotation
{
    public class RotationCommand : IRotationCommand
    {
        public EntityCommand Command => EntityCommand.SetRotation;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private Vector3 rotation;
        private bool blend;

        /// <summary>
        /// Initialise command with the specified <see cref="Vector3"/> rotation and blend.
        /// </summary>
        public void Initialise(Vector3 rotation, bool blend)
        {
            this.rotation = rotation;
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
                Model   = new SetRotationCommand
                {
                    Rotation = rotation,
                    Blend    = blend
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        public Vector3 GetRotation()
        {
            return rotation;
        }
    }
}
