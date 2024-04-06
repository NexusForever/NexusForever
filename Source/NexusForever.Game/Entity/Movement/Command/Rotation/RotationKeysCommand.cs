using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Entity.Movement.Key;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Rotation
{
    public class RotationKeysCommand : IRotationCommand
    {
        public EntityCommand Command => EntityCommand.SetRotationKeys;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => rotationKeys?.IsFinalised ?? false;

        private RotationKeys rotationKeys;

        /// <summary>
        /// Initialise command with the specified times and <see cref="Vector3"/> rotation key values.
        /// </summary>
        public void Initialise(IMovementManager movementManager, List<uint> times, List<Vector3> rotations)
        {
            rotationKeys = new RotationKeys();
            rotationKeys.Initialise(movementManager, times, rotations);
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
                Model   = new SetRotationKeysCommand
                {
                    Times     = rotationKeys.Times,
                    Rotations = rotationKeys.Values,
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        public Vector3 GetRotation()
        {
            return rotationKeys.GetInterpolated();
        }
    }
}
