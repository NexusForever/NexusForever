using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Rotation
{
    public class RotationFacePositionCommand : IRotationCommand
    {
        public EntityCommand Command => EntityCommand.SetRotationFacePosition;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private IMovementManager movementManager;
        private Vector3 facePosition;

        /// <summary>
        /// 
        /// </summary>
        public void Initialise(IMovementManager movementManager, Vector3 facePosition)
        {
            this.movementManager = movementManager;
            this.facePosition    = facePosition;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model   = new SetRotationFacePositionCommand
                {
                    Position = facePosition
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        public Vector3 GetRotation()
        {
            Vector3 vector = Vector3.Normalize(facePosition - movementManager.GetPosition());
            float pitch = MathF.Asin(-vector.Y);
            float yaw = MathF.Atan2(-vector.X, -vector.Z);
            return new Vector3(yaw, pitch, 0f);
        }
    }
}
