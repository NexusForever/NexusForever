using NexusForever.Game.Abstract.Entity.Movement;
using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;

namespace NexusForever.Game.Entity.Movement.Command.Rotation
{
    public class RotationFaceUnitCommand : IRotationCommand
    {
        public EntityCommand Command => EntityCommand.SetRotationFaceUnit;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private IMovementManager movementManager;
        private uint faceUnitId;

        /// <summary>
        /// Initialise command with the specified face unit id.
        /// </summary>
        public void Initialise(IMovementManager movementManager, uint faceUnitId)
        {
            this.movementManager = movementManager;
            this.faceUnitId      = faceUnitId;
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
                Command    = Command,
                Model = new SetRotationFaceUnitCommand
                {
                    UnitId = faceUnitId
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        public Vector3 GetRotation()
        {
            IWorldEntity entity = movementManager.Owner.Map.GetEntity<IWorldEntity>(faceUnitId);
            if (entity == null)
                return Vector3.Zero;

            Vector3 vector = Vector3.Normalize(entity.MovementManager.GetPosition() - movementManager.GetPosition());
            float yaw = MathF.Atan2(-vector.X, -vector.Z);

            // RotationFaceUnitCommand is only yaw regardless of mode
            return new Vector3(yaw, 0f, 0f);
        }
    }
}
