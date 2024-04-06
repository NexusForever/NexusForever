using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.AntiTamper;
using NexusForever.Game.Abstract.Entity.Movement.Command;
using NexusForever.Game.Abstract.Entity.Movement.Command.Mode;
using NexusForever.Game.Abstract.Entity.Movement.Command.Move;
using NexusForever.Game.Abstract.Entity.Movement.Command.Platform;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Abstract.Entity.Movement.Command.Scale;
using NexusForever.Game.Abstract.Entity.Movement.Command.State;
using NexusForever.Game.Abstract.Entity.Movement.Command.Time;
using NexusForever.Game.Abstract.Entity.Movement.Command.Velocity;
using NexusForever.Game.Abstract.Entity.Movement.Generator;
using NexusForever.Game.Entity.Movement.Generator;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Command.State;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Network;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity.Movement
{
    public class MovementManager : IMovementManager
    {
        /// <summary>
        /// Owner <see cref="IWorldEntity"/> for the <see cref="IMovementManager"/>.
        /// </summary>
        public IWorldEntity Owner { get; private set; }

        /// <summary>
        /// Determines if one or more groups have been modified and need to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if an entity command requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => timeCommandGroup.RequiresSynchronisation
            || platformCommandGroup.RequiresSynchronisation
            || positionCommandGroup.RequiresSynchronisation
            || velocityCommandGroup.RequiresSynchronisation
            || moveCommandGroup.RequiresSynchronisation
            || rotationCommandGroup.RequiresSynchronisation
            || scaleCommandGroup.RequiresSynchronisation
            || stateCommandGroup.RequiresSynchronisation
            || modeCommandGroup.RequiresSynchronisation;

        /// <summary>
        /// Determins if the <see cref="IWorldEntity"/> owner is being controlled by the server.
        /// </summary>
        /// <remarks>
        /// This can be false if a player is controlling the entity.
        /// </remarks>
        public bool ServerControl
        {
            get => serverControl;
            set
            {
                serverControl = value;

                if (!serverControl)
                    Finalise();
            }
        }

        private bool serverControl = true;

        #region Dependency Injection

        private readonly ITimeCommandGroup timeCommandGroup;
        private readonly IPlatformCommandGroup platformCommandGroup;
        private readonly IPositionCommandGroup positionCommandGroup;
        private readonly IVelocityCommandGroup velocityCommandGroup;
        private readonly IMoveCommandGroup moveCommandGroup;
        private readonly IRotationCommandGroup rotationCommandGroup;
        private readonly IScaleCommandGroup scaleCommandGroup;
        private readonly IStateCommandGroup stateCommandGroup;
        private readonly IModeCommandGroup modeCommandGroup;

        private readonly IClientMovementCommandValidator commandValidator;

        public MovementManager(
            ITimeCommandGroup timeCommandGroup,
            IPlatformCommandGroup platformCommandGroup,
            IPositionCommandGroup positionCommandGroup,
            IVelocityCommandGroup velocityCommandGroup,
            IMoveCommandGroup moveCommandGroup,
            IRotationCommandGroup rotationCommandGroup,
            IScaleCommandGroup scaleCommandGroup,
            IStateCommandGroup stateCommandGroup,
            IModeCommandGroup modeCommandGroup,
            IClientMovementCommandValidator commandValidator)
        {
            this.timeCommandGroup     = timeCommandGroup;
            this.platformCommandGroup = platformCommandGroup;
            this.positionCommandGroup = positionCommandGroup;
            this.velocityCommandGroup = velocityCommandGroup;
            this.moveCommandGroup     = moveCommandGroup;
            this.rotationCommandGroup = rotationCommandGroup;
            this.scaleCommandGroup    = scaleCommandGroup;
            this.stateCommandGroup    = stateCommandGroup;
            this.modeCommandGroup     = modeCommandGroup;

            this.commandValidator     = commandValidator;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMovementManager"/> with owner <see cref="IWorldEntity"/>.
        /// </summary>
        public void Initialise(IWorldEntity entity)
        {
            Owner = entity;

            timeCommandGroup.Initialise();
            platformCommandGroup.Initialise();
            positionCommandGroup.Initialise(this);
            velocityCommandGroup.Initialise();
            moveCommandGroup.Initialise(this);
            rotationCommandGroup.Initialise(this, positionCommandGroup);
            scaleCommandGroup.Initialise(this);
            stateCommandGroup.Initialise(this);
            modeCommandGroup.Initialise(this);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (Owner.Map == null)
                return;

            void UpdateEntityCommandGroup(IEntityCommandGroup commandGroup)
            {
                commandGroup.Update(lastTick);

                if (commandGroup.IsDirty)
                {
                    commandGroup.IsDirty = false;
                    IsDirty = true;
                }
            }

            UpdateEntityCommandGroup(timeCommandGroup);
            UpdateEntityCommandGroup(platformCommandGroup);
            UpdateEntityCommandGroup(positionCommandGroup);
            UpdateEntityCommandGroup(velocityCommandGroup);
            UpdateEntityCommandGroup(moveCommandGroup);
            UpdateEntityCommandGroup(rotationCommandGroup);
            UpdateEntityCommandGroup(scaleCommandGroup);
            UpdateEntityCommandGroup(stateCommandGroup);
            UpdateEntityCommandGroup(modeCommandGroup);

            BroadcastNetworkEntityCommands();
        }

        /// <summary>
        /// Return the default entity commands if schronisation is required.
        /// </summary>
        /// <remarks>
        /// The true entity commands will be send to the client after loading.
        /// </remarks>
        public IEnumerable<INetworkEntityCommand> GetInitialNetworkEntityCommands()
        {
            var commands = new List<INetworkEntityCommand>
            {
                platformCommandGroup.GetDefaultNetworkEntityCommand(),
                positionCommandGroup.GetDefaultNetworkEntityCommand(),
                velocityCommandGroup.GetDefaultNetworkEntityCommand(),
                moveCommandGroup.GetDefaultNetworkEntityCommand(),
                rotationCommandGroup.GetDefaultNetworkEntityCommand(),
                scaleCommandGroup.GetDefaultNetworkEntityCommand(),
                stateCommandGroup.GetDefaultNetworkEntityCommand(),
                modeCommandGroup.GetDefaultNetworkEntityCommand()
            };

            return commands;
        }

        /// <summary>
        /// Return the current entity commands.
        /// </summary>
        public IEnumerable<INetworkEntityCommand> GetNetworkEntityCommands()
        {
            static void AddNetworkEntityCommand(List<INetworkEntityCommand> commands, INetworkEntityCommand command)
            {
                if (command != null)
                    commands.Add(command);
            }

            var commands = new List<INetworkEntityCommand>();
            //AddNetworkEntityCommand(commands, timeCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, platformCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, positionCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, velocityCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, moveCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, rotationCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, scaleCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, stateCommandGroup.GetNetworkEntityCommand());
            AddNetworkEntityCommand(commands, modeCommandGroup.GetNetworkEntityCommand());

            return commands;
        }

        private ServerEntityCommand BuildNetworkEntityCommands()
        {
            var serverEntityCommand = new ServerEntityCommand
            {
                Guid             = Owner.Guid,
                Time             = GetTime(),
                TimeReset        = timeCommandGroup.TimeReset,
                ServerControlled = ServerControl,
                Commands         = GetNetworkEntityCommands().ToList()
            };

            return serverEntityCommand;
        }

        /// <summary>
        /// Send current entity commands to supplied <see cref="IGameSession"/>.
        /// </summary>
        public void SendNetworkEntityCommands(IGameSession session)
        {
            session.EnqueueMessageEncrypted(BuildNetworkEntityCommands());
        }

        /// <summary>
        /// Broadcast current entitycommands if changes have occured since the last broadcast.
        /// </summary>
        public void BroadcastNetworkEntityCommands()
        {
            if (!IsDirty)
                return;

            // only send commands to self if server controlled
            Owner.EnqueueToVisible(BuildNetworkEntityCommands(), ServerControl);

            IsDirty                    = false;
            timeCommandGroup.TimeReset = false;
        }

        /// <summary>
        /// Process incoming entity commands from client.
        /// </summary>
        public void HandleClientEntityCommands(IEnumerable<INetworkEntityCommand> commands, uint time)
        {
            if (ServerControl)
                return;

            foreach (INetworkEntityCommand command in commands)
            {
                switch (command.Model)
                {
                    case SetTimeCommand setTime:
                        commandValidator.ValidateTime(setTime.Time, GetTime());
                        break;
                    case SetPositionCommand setPosition:
                    {
                        commandValidator.ValidatePosition();
                        SetPosition(setPosition.Position, setPosition.Blend);
                        break;
                    }
                    case SetVelocityCommand setVelocity:
                        SetVelocity(setVelocity.Velocity, setVelocity.Blend);
                        break;
                    case SetMoveCommand setMove:
                        SetMove(setMove.Move, setMove.Blend);
                        break;
                    case SetRotationCommand setRotation:
                        SetRotation(setRotation.Rotation, setRotation.Blend);
                        break;
                    case SetStateCommand setState:
                    {
                        commandValidator.ValidateState();
                        SetState(setState.State);
                        break;
                    }
                    case SetModeCommand setMode:
                    {
                        commandValidator.ValidateMode();
                        SetMode(setMode.Mode);
                        break;
                    }
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Finalise the current entity command for each group.
        /// </summary>
        public void Finalise()
        {
            timeCommandGroup.Finalise();
            platformCommandGroup.Finalise();
            // finalise rotation before position to ensure the correct rotation is used for the position
            rotationCommandGroup.Finalise();
            positionCommandGroup.Finalise();
            velocityCommandGroup.Finalise();
            moveCommandGroup.Finalise();
            scaleCommandGroup.Finalise();
            stateCommandGroup.Finalise();
            modeCommandGroup.Finalise();
        }

        /// <summary>
        /// Return the current server time.
        /// </summary>
        public uint GetTime()
        {
            return timeCommandGroup.GetTime();
        }
        
        /// <summary>
        /// Reset the current server time to 0.
        /// </summary>
        /// <remarks>
        /// This will reset the time synchronisation information at the client.
        /// </remarks>
        public void ResetTime()
        {
            timeCommandGroup.ResetTime();
            IsDirty = true;
        }

        /// <summary>
        /// Return current platform unit id.
        /// </summary>
        public uint? GetPlatform()
        {
            return platformCommandGroup.GetPlatform();
        }

        /// <summary>
        /// Set platform with supplied unit id.
        /// </summary>
        public void SetPlatform(uint? platformUnitId)
        {
            platformCommandGroup.SetPlatform(platformUnitId);
        }

        /// <summary>
        /// Return the current position.
        /// </summary>
        public Vector3 GetPosition()
        {
            return positionCommandGroup.GetPosition();
        }

        /// <summary>
        /// Set position with the supplied <see cref="Vector3"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that this position doesn't always match the grid position (eg: when on a platform/vehicle)
        /// </remarks>
        public void SetPosition(Vector3 position, bool blend)
        {
            positionCommandGroup.SetPosition(position, blend);
        }

        /// <summary>
        /// Set position with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        public void SetPositionKeys(List<uint> times, List<Vector3> positions)
        {
            if (!ServerControl)
                return;

            positionCommandGroup.SetPositionKeys(times, positions);
        }

        /// <summary>
        /// Launch a new custom spline with supplied nodes, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        public void SetPositionPath(List<Vector3> nodes, SplineType type, SplineMode mode, float speed)
        {
            if (!ServerControl)
                return;

            positionCommandGroup.SetPositionPath(nodes, type, mode, speed);
        }

        /// <summary>
        /// Launch a new single spline with supplied <see cref="SplineMode"/> and speed.
        /// </summary>
        public void SetPositionSpline(ushort splineId, SplineMode mode, float speed)
        {
            if (!ServerControl)
                return;

            positionCommandGroup.SetPositionSpline(splineId, mode, speed);
        }

        /// <summary>
        /// Launch a new multi spline with supplied <see cref="SplineMode"/> and speed.
        /// </summary>
        public void SetPositionMultiSpline(List<ushort> splineIds, SplineMode mode, float speed)
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

        /// <summary>
        /// Return the current velocity.
        /// </summary>
        public Vector3 GetVelocity()
        {
            return velocityCommandGroup.GetVelocity();
        }

        /// <summary>
        /// Set velocity with the supplied <see cref="Vector3"/>.
        /// </summary>
        public void SetVelocity(Vector3 velocity, bool blend)
        {
            velocityCommandGroup.SetVelocity(velocity, blend);
        }

        /// <summary>
        /// Set velocity with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        public void SetVelocityKeys()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set velocity with the default values.
        /// </summary>
        public void SetVelocityDefaults()
        {
            if (!ServerControl)
                return;

            velocityCommandGroup.SetVelocityDefaults();
        }

        /// <summary>
        /// Return the current move direction.
        /// </summary>
        public Vector3 GetMove()
        {
            return moveCommandGroup.GetMove();
        }

        /// <summary>
        /// Set move direction with the supplied <see cref="Vector3"/>.
        /// </summary>
        public void SetMove(Vector3 move, bool blend)
        {
            moveCommandGroup.SetMove(move, blend);
        }

        /// <summary>
        /// Set move direction with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        public void SetMoveKeys(List<uint> times, List<Vector3> moves)
        {
            if (!ServerControl)
                return;

            moveCommandGroup.SetMoveKeys(times, moves);
        }

        /// <summary>
        /// Set move direction with the default values.
        /// </summary>
        public void SetMoveDefaults(bool blend)
        {
            if (!ServerControl)
                return;

            moveCommandGroup.SetMoveDefaults(blend);
        }

        /// <summary>
        /// Return the current rotation.
        /// </summary>
        public Vector3 GetRotation()
        {
            return rotationCommandGroup.GetRotation();
        }

        /// <summary>
        /// Set rotation with the supplied rotation <see cref="Vector3"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that this rotation doesn't always match the entity rotation (eg: when on a vehicle)
        /// </remarks>
        public void SetRotation(Vector3 rotation, bool blend)
        {
            rotationCommandGroup.SetRotation(rotation, blend);
        }

        /// <summary>
        /// Set rotation with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        public void SetRotationKeys(List<uint> times, List<Vector3> rotations)
        {
            if (!ServerControl)
                return;

            rotationCommandGroup.SetRotationKeys(times, rotations);
        }

        /// <summary>
        /// NYI
        /// </summary>
        public void SetRotationSpline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// NYI
        /// </summary>
        public void SetRotationMultiSpline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set rotation to face the supplied unit id.
        /// </summary>
        public void SetRotationFaceUnit(uint faceUnit)
        {
            if (!ServerControl)
                return;

            rotationCommandGroup.SetRotationFaceUnit(faceUnit);
        }

        /// <summary>
        /// Set rotation to face the supplied <see cref="Vector3"/> position.
        /// </summary>
        public void SetRotationFacePosition(Vector3 position)
        {
            if (!ServerControl)
                return;

            rotationCommandGroup.SetRotationFacePosition(position);
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
            if (!ServerControl)
                return;

            rotationCommandGroup.SetRotationSpin(rotation, duration, spin);
        }

        /// <summary>
        /// Set rotation with the default values.
        /// </summary>
        /// <remarks>
        /// Rotation will be the direction of movement of paths, splines or keys.
        /// </remarks>
        public void SetRotationDefaults()
        {
            if (!ServerControl)
                return;

            rotationCommandGroup.SetRotationDefaults();
        }

        /// <summary>
        /// Return the current scale.
        /// </summary>
        public float GetScale()
        {
            return scaleCommandGroup.GetScale();
        }

        /// <summary>
        /// Set scale with the supplied value.
        /// </summary>
        public void SetScale(float scale)
        {
            if (!ServerControl)
                return;

            scaleCommandGroup.SetScale(scale);
        }

        /// <summary>
        /// Set scale with the supplied value key and time values.
        /// </summary>
        public void SetScaleKeys(List<uint> times, List<float> scales)
        {
            if (!ServerControl)
                return;

            scaleCommandGroup.SetScaleKeys(times, scales);
        }

        /// <summary>
        /// Reurn the current <see cref="StateFlags"/>.
        /// </summary>
        public StateFlags GetState()
        {
            return stateCommandGroup.GetState();
        }

        /// <summary>
        /// Set the state flags with the supplied <see cref="StateFlags"/>.
        /// </summary>
        public void SetState(StateFlags state)
        {
            stateCommandGroup.SetState(state);
        }

        /// <summary>
        /// Set the state flags with the supplied <see cref="StateFlags"/> key and time values.
        /// </summary>
        public void SetStateKeys(List<uint> times, List<StateFlags> states)
        {
            if (!ServerControl)
                return;

            stateCommandGroup.SetStateKeys(times, states);
        }

        /// <summary>
        /// Set the <see cref="StateFlags"/> with the default values.
        /// </summary>
        public void SetStateDefault()
        {
            if (!ServerControl)
                return;

            stateCommandGroup.SetStateDefault();
        }

        /// <summary>
        /// Return the current <see cref="ModeType"/>.
        /// </summary>
        public ModeType GetMode()
        {
            return modeCommandGroup.GetMode();
        }

        /// <summary>
        /// Set the <see cref="ModeType"/>.
        /// </summary>
        public void SetMode(ModeType mode)
        {
            modeCommandGroup.SetMode(mode);
        }

        /// <summary>
        /// Set the <see cref="ModeType"/> with the supplied <see cref="ModeType"/> key and time values.
        /// </summary>
        public void SetModeKeys(List<uint> times, List<ModeType> modes)
        {
            if (!ServerControl)
                return;

            modeCommandGroup.SetModeKeys(times, modes);
        }

        /// <summary>
        /// Set the <see cref="ModeType"/> with the default values.
        /// </summary>
        public void SetModeDefault()
        {
            if (!ServerControl)
                return;

            modeCommandGroup.SetModeDefault();
        }

        /// <summary>
        /// Launch a new spline with the supplied nodes, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        public void LaunchSpline(List<Vector3> nodes, SplineType type, SplineMode mode, float speed)
        {
            if (!ServerControl)
                return;

            SetState(StateFlags.Move);
            SetMoveDefaults(false);
            SetRotationDefaults();

            SetPositionPath(nodes, type, mode, speed);
        }

        /// <summary>
        /// Launch a new spline with the supplied spline id, <see cref="SplineMode"/> and speed.
        /// </summary>
        /// <remarks>
        /// Specify <paramref name="rotation"/> to use the rotation values in the tbl.
        /// </remarks>
        public void LaunchSpline(ushort splineId, SplineMode mode, float speed, bool rotation)
        {
            if (!ServerControl)
                return;

            SetState(StateFlags.Move);
            SetMoveDefaults(false);

            // TODO: implement spline rotation
            /*if (rotation)
                SetRotationSpline();
            else*/
                SetRotationDefaults();

            SetPositionSpline(splineId, mode, speed);
        }

        /// <summary>
        /// Launch a new custom linear spline where the points are generated by <see cref="IMovementGenerator"/>.
        /// </summary>
        public void LaunchGenerator(IMovementGenerator generator, float speed, SplineMode mode = SplineMode.OneShot)
        {
            if (!ServerControl)
                return;

            List<Vector3> nodes = generator.CalculatePath();
            LaunchSpline(nodes, SplineType.Linear, mode, speed);
        }

        /// <summary>
        /// Launch a new follow spline, following the supplied <see cref="IWorldEntity"/> at distance.
        /// </summary>
        public void Follow(IWorldEntity entity, float distance)
        {
            if (!ServerControl)
                return;

            SetState(StateFlags.Move);
            SetMoveDefaults(false);
            SetRotationFaceUnit(entity.Guid);

            // angle is directly behind entity being followed
            float angle = -entity.Rotation.X;
            angle += MathF.PI / 2;

            var generator = new DirectMovementGenerator
            {
                Begin = positionCommandGroup.GetPosition(),
                Final = entity.Position.GetPoint2D(angle, distance),
                Map   = entity.Map
            };

            // TODO: calculate speed based on entity being followed.
            List<Vector3> nodes = generator.CalculatePath();
            SetPositionPath(nodes, SplineType.Linear, SplineMode.OneShot, 8f);
        }
    }
}
