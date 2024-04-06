using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Network.World.Entity;
using NexusForever.Script.Template;
using NexusForever.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity.Movement.Command.Position
{
    public class PositionCommandGroup : IPositionCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => Command.Command
            is EntityCommand.SetPositionKeys
            or EntityCommand.SetPositionPath
            or EntityCommand.SetPositionSpline
            or EntityCommand.SetPositionMultiSpline;

        /// <summary>
        /// Current position entity command.
        /// </summary>
        public IPositionCommand Command { get; private set; }

        private readonly UpdateTimer relocationTimer = new(TimeSpan.FromSeconds(1));
        private Vector3 lastPosition;

        private IMovementManager movementManager;

        #region Dependency Injection

        private readonly IFactoryInterface<IPositionCommand> factory;

        public PositionCommandGroup(
            IFactoryInterface<IPositionCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IPositionCommandGroup"/ with default command.
        /// </summary>
        public void Initialise(IMovementManager movementManager)
        {
            this.movementManager = movementManager;

            SetPosition(Vector3.Zero, false);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (Command == null)
                return;

            Command.Update(lastTick);

            relocationTimer.Update(lastTick);
            if (relocationTimer.HasElapsed)
            {
                Relocate();
                relocationTimer.Reset();
            }

            if (Command.IsFinalised)
            {
                movementManager.Owner.InvokeScriptCollection<IWorldEntityScript>(s => s.OnPositionEntityCommandFinalise(Command));
                Finalise();
            }
        }

        private void Relocate()
        {
            Vector3 position = GetPosition();

            uint? platformUnitId = movementManager.GetPlatform();
            if (platformUnitId != null)
            {
                IWorldEntity platformEntity = movementManager.Owner.Map.GetEntity<IWorldEntity>(platformUnitId.Value);
                if (platformEntity != null)
                    position += platformEntity.Position;
            }

            if (lastPosition == position)
                return;

            lastPosition = position;
            movementManager.Owner.Relocate(position);
        }

        /// <summary>
        /// Return the default <see cref="INetworkEntityCommand"/> for the entity command group.
        /// </summary>
        /// <remarks>
        /// The default command to send if the entity requires synchronisation.
        /// </remarks>
        public INetworkEntityCommand GetDefaultNetworkEntityCommand()
        {
            var command = factory.Resolve<PositionCommand>();
            command.Initialise(GetPosition(), false);
            return command.GetNetworkEntityCommand();
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command group.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return Command.GetNetworkEntityCommand();
        }

        /// <summary>
        /// Finalise the current entity command.
        /// </summary>
        public void Finalise()
        {
            if (Command == null)
                return;

            Vector3 position = GetPosition();
            Command = null;

            SetPosition(position, true);
        }

        /// <summary>
        /// Get the current <see cref="Vector3"/> position value.
        /// </summary>
        public Vector3 GetPosition()
        {
            Vector3 position = Command.GetPosition();

            // add "float" height for modes 1 and 3
            if (movementManager.GetMode() is ModeType.Swim or ModeType.Free)
                position += new Vector3(0f, 1.5707964f, 0f);

            return position;
        }

        /// <summary>
        /// Set the position to the supplied <see cref="Vector3"/> value.
        /// </summary>
        public void SetPosition(Vector3 position, bool blend)
        {
            Finalise();

            var command = factory.Resolve<PositionCommand>();
            command.Initialise(position, blend);
            Command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set the position to the interpolated <see cref="Vector3"/> between the supplied times and positions.
        /// </summary> 
        public void SetPositionKeys(List<uint> times, List<Vector3> positions)
        {
            Finalise();

            var command = factory.Resolve<PositionKeysCommand>();
            command.Initialise(movementManager, times, positions);
            Command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set the position based on the supplied nodes, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        public void SetPositionPath(List<Vector3> nodes, SplineType type, SplineMode mode, float speed)
        {
            Finalise();

            var command = factory.Resolve<PositionPathCommand>();
            command.Initialise(nodes, type, mode, speed);
            Command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set the position based on the supplied spline, <see cref="SplineMode"/> and speed.
        /// </summary>
        public void SetPositionSpline(ushort splineId, SplineMode mode, float speed)
        {
            Finalise();

            var command = factory.Resolve<PositionSplineCommand>();
            command.Initialise(splineId, mode, speed);
            Command = command;

            IsDirty = true;
        }

        /// <summary>
        /// NYI
        /// </summary>
        public void SetPositionMultiSpline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// NYI
        /// </summary>
        public void SetPositionProjectile()
        {
            throw new NotImplementedException();
        }
    }
}
