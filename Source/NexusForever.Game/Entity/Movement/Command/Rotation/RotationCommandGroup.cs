using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.Rotation
{
    public class RotationCommandGroup : IRotationCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => command.Command
            is EntityCommand.SetRotationKeys
            or EntityCommand.SetRotationSpline
            or EntityCommand.SetPositionMultiSpline
            or EntityCommand.SetRotationSpin;

        private IRotationCommand command;

        private IMovementManager movementManager;
        private IPositionCommandGroup positionCommandGroup;

        #region Dependency Injection

        private readonly IFactoryInterface<IRotationCommand> factory;

        public RotationCommandGroup(
            IFactoryInterface<IRotationCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IRotationCommandGroup"/ with default command.
        /// </summary>
        public void Initialise(IMovementManager movementManager, IPositionCommandGroup positionCommandGroup)
        {
            this.movementManager      = movementManager;
            this.positionCommandGroup = positionCommandGroup;

            SetRotation(Vector3.Zero, false);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (command == null)
                return;

            command.Update(lastTick);

            if (command.IsFinalised)
                Finalise();
        }

        /// <summary>
        /// Return the default <see cref="INetworkEntityCommand"/> for the entity command group.
        /// </summary>
        /// <remarks>
        /// The default command to send if the entity requires synchronisation.
        /// </remarks>
        public INetworkEntityCommand GetDefaultNetworkEntityCommand()
        {
            var command = factory.Resolve<RotationCommand>();
            command.Initialise(GetRotation(), false);
            return command.GetNetworkEntityCommand();
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command group.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return command.GetNetworkEntityCommand();
        }

        /// <summary>
        /// Finalise the current entity command.
        /// </summary>
        public void Finalise()
        {
            if (command == null)
                return;

            Vector3 rotation = GetRotation();
            command = null;

            SetRotation(rotation, false);
        }

        /// <summary>
        /// Get the current <see cref="Vector3"/> rotation value.
        /// </summary>
        public Vector3 GetRotation()
        {
            return command.GetRotation();
        }

        /// <summary>
        /// Set rotation to the supplied <see cref="Vector3"/> value.
        /// </summary>
        public void SetRotation(Vector3 rotation, bool blend)
        {
            Finalise();

            var command = factory.Resolve<RotationCommand>();
            command.Initialise(rotation, blend);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set rotation to the interpolated value between the supplied times and rotations.
        /// </summary>
        public void SetRotationKeys(List<uint> times, List<Vector3> rotations)
        {
            Finalise();

            var command = factory.Resolve<RotationKeysCommand>();
            command.Initialise(movementManager, times, rotations);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// NYI
        /// </summary>
        public void SetRotationSpline()
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// NYI
        /// </summary>
        public void SetRotationMultiSpline()
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set rotation value to face the supplied entity.
        /// </summary>
        public void SetRotationFaceUnit(uint faceUnitId)
        {
            Finalise();

            var command = factory.Resolve<RotationFaceUnitCommand>();
            command.Initialise(movementManager, faceUnitId);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set rotation value to face the supplied position.
        /// </summary>
        public void SetRotationFacePosition(Vector3 position)
        {
            Finalise();

            var command = factory.Resolve<RotationFacePositionCommand>();
            command.Initialise(movementManager, position);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set the rotation value to the supplied spin.
        /// </summary>
        /// <remarks>
        /// <paramref name="rotation"/> is the initial rotation value.
        /// <paramref name="spin"/> is the amount of radians to spin per second.
        /// </remarks>
        public void SetRotationSpin(Vector3 rotation, TimeSpan duration, float spin)
        {
            Finalise();

            var command = factory.Resolve<RotationSpinCommand>();
            command.Initialise(rotation, duration, spin);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set rotation with the default values.
        /// </summary>
        /// <remarks>
        /// Rotation will be the direction of movement of paths, splines or keys.
        /// </remarks>
        public void SetRotationDefaults()
        {
            Finalise();

            var command = factory.Resolve<RotationDefaultsCommand>();
            command.Initialise(GetRotation(), positionCommandGroup);
            this.command = command;

            IsDirty = true;
        }
    }
}
