using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Generator;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Command.State;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Network;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity.Movement
{
    public interface IMovementManager : IUpdate
    {
        /// <summary>
        /// Owner <see cref="IWorldEntity"/> for the <see cref="IMovementManager"/>.
        /// </summary>
        public IWorldEntity Owner { get; }

        /// <summary>
        /// Determines if one or more groups have been modified and need to be sent to the client.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Determines if an entity command requires resynchronisation after loading.
        /// </summary>
        bool RequiresSynchronisation { get; }

        /// <summary>
        /// Determins if the <see cref="IWorldEntity"/> owner is being controlled by the server.
        /// </summary>
        /// <remarks>
        /// This can be false if a player is controlling the entity.
        /// </remarks>
        bool ServerControl { get; set; }

        /// <summary>
        /// Initialise <see cref="IMovementManager"/> with owner <see cref="IWorldEntity"/>.
        /// </summary>
        void Initialise(IWorldEntity entity);

        /// <summary>
        /// Return the default entity commands if schronisation is required.
        /// </summary>
        /// <remarks>
        /// The true entity commands will be send to the client after loading.
        /// </remarks>
        IEnumerable<INetworkEntityCommand> GetInitialNetworkEntityCommands();

        /// <summary>
        /// Return the current entity commands.
        /// </summary>
        IEnumerable<INetworkEntityCommand> GetNetworkEntityCommands();

        /// <summary>
        /// Send current entity commands to supplied <see cref="IGameSession"/>.
        /// </summary>
        void SendNetworkEntityCommands(IGameSession session);

        /// <summary>
        /// Broadcast current entity commands if changes have occured since the last broadcast.
        /// </summary>
        void BroadcastNetworkEntityCommands();

        /// <summary>
        /// Process incoming entity commands from client.
        /// </summary>
        void HandleClientEntityCommands(IEnumerable<INetworkEntityCommand> commands, uint time);

        /// <summary>
        /// Finalise the current entity command for each group.
        /// </summary>
        public void Finalise();

        /// <summary>
        /// Return the current server time.
        /// </summary>
        uint GetTime();

        /// <summary>
        /// Reset the current server time to 0.
        /// </summary>
        /// <remarks>
        /// This will reset the time synchronisation information at the client.
        /// </remarks>
        void ResetTime();

        /// <summary>
        /// Return current platform unit id.
        /// </summary>
        uint? GetPlatform();

        /// <summary>
        /// Set platform with supplied unit id.
        /// </summary>
        void SetPlatform(uint? platformUnitId);

        /// <summary>
        /// Return the current position.
        /// </summary>
        Vector3 GetPosition();

        /// <summary>
        /// Set position with the supplied <see cref="Vector3"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that this position doesn't always match the grid position (eg: when on a platform/vehicle)
        /// </remarks>
        void SetPosition(Vector3 position, bool blend);

        /// <summary>
        /// Set position with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        void SetPositionKeys(List<uint> times, List<Vector3> positions);

        /// <summary>
        /// Launch a new custom spline with supplied nodes, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        void SetPositionPath(List<Vector3> nodes, SplineType type, SplineMode mode, float speed);

        /// <summary>
        /// Launch a new single spline with supplied <see cref="SplineMode"/> and speed.
        /// </summary>
        void SetPositionSpline(ushort splineId, SplineMode mode, float speed);

        /// <summary>
        /// Launch a new multi spline with supplied <see cref="SplineMode"/> and speed.
        /// </summary>
        void SetPositionMultiSpline(List<ushort> splineIds, SplineMode mode, float speed);

        /// <summary>
        /// NYI
        /// </summary>
        void SetPositionProjectile();

        /// <summary>
        /// Return the current velocity.
        /// </summary>
        Vector3 GetVelocity();

        /// <summary>
        /// Set velocity with the supplied <see cref="Vector3"/>.
        /// </summary>
        void SetVelocity(Vector3 velocity, bool blend);

        /// <summary>
        /// Set velocity with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        void SetVelocityKeys();

        /// <summary>
        /// Set velocity with the default values.
        /// </summary>
        void SetVelocityDefaults();

        /// <summary>
        /// Return the current move direction.
        /// </summary>
        Vector3 GetMove();

        /// <summary>
        /// Set move direction with the supplied <see cref="Vector3"/>.
        /// </summary>
        void SetMove(Vector3 move, bool blend);

        /// <summary>
        /// Set move direction with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        void SetMoveKeys(List<uint> times, List<Vector3> moves);

        /// <summary>
        /// Set move direction with the default values.
        /// </summary>
        void SetMoveDefaults(bool blend);

        /// <summary>
        /// Return the current rotation.
        /// </summary>
        Vector3 GetRotation();

        /// <summary>
        /// Set rotation with supplied <see cref="Vector3"/>.
        /// </summary>
        /// <remarks>
        /// Be aware that this rotation doesn't always match the entity rotation (eg: when on a vehicle)
        /// </remarks>
        void SetRotation(Vector3 rotation, bool blend);

        /// <summary>
        /// Set rotation with the supplied <see cref="Vector3"/> key and time values.
        /// </summary>
        void SetRotationKeys(List<uint> times, List<Vector3> rotations);

        /// <summary>
        /// NYI
        /// </summary>
        void SetRotationSpline();

        /// <summary>
        /// NYI
        /// </summary>
        void SetRotationMultiSpline();

        /// <summary>
        /// Set rotation to face the supplied unit id.
        /// </summary>
        void SetRotationFaceUnit(uint faceUnit);

        /// <summary>
        /// Set rotation to face the supplied <see cref="Vector3"/> position.
        /// </summary>
        void SetRotationFacePosition(Vector3 position);

        /// <summary>
        /// Set the rotation value to the supplied spin.
        /// </summary>
        /// <remarks>
        /// <paramref name="rotation"/> is the initial rotation value.
        /// <paramref name="spin"/> is the amount of radians to spin per second.
        /// </remarks>
        void SetRotationSpin(Vector3 rotation, TimeSpan duration, float spin);

        /// <summary>
        /// Set rotation with the default values.
        /// </summary>
        /// <remarks>
        /// Rotation will be the direction of movement of paths, splines or keys.
        /// </remarks>
        void SetRotationDefaults();

        /// <summary>
        /// Return the current scale.
        /// </summary>
        float GetScale();

        /// <summary>
        /// Set scale with the supplied value.
        /// </summary>
        void SetScale(float scale);

        /// <summary>
        /// Set scale with the supplied value key and time values.
        /// </summary>
        void SetScaleKeys(List<uint> times, List<float> scales);

        /// <summary>
        /// Reurn the current <see cref="StateFlags"/>.
        /// </summary>
        StateFlags GetState();

        /// <summary>
        /// Set the state flags with the supplied <see cref="StateFlags"/>.
        /// </summary>
        void SetState(StateFlags state);

        /// <summary>
        /// Set the state flags with the supplied <see cref="StateFlags"/> key and time values.
        /// </summary>
        void SetStateKeys(List<uint> times, List<StateFlags> states);

        /// <summary>
        /// Set the <see cref="StateFlags"/> with the default values.
        /// </summary>
        void SetStateDefault();

        /// <summary>
        /// Return the current <see cref="ModeType"/>.
        /// </summary>
        ModeType GetMode();

        /// <summary>
        /// Set the <see cref="ModeType"/>.
        /// </summary>
        void SetMode(ModeType mode);

        /// <summary>
        /// Set the <see cref="ModeType"/> with the supplied <see cref="ModeType"/> key and time values.
        /// </summary>
        void SetModeKeys(List<uint> times, List<ModeType> modes);

        /// <summary>
        /// Set the <see cref="ModeType"/> with the default values.
        /// </summary>
        void SetModeDefault();

        /// <summary>
        /// Launch a new spline with the supplied nodes, <see cref="SplineType"/>, <see cref="SplineMode"/> and speed.
        /// </summary>
        void LaunchSpline(List<Vector3> nodes, SplineType type, SplineMode mode, float speed);

        /// <summary>
        /// Launch a new spline with the supplied spline id, <see cref="SplineMode"/> and speed.
        /// </summary>
        /// <remarks>
        /// Specify <paramref name="rotation"/> to use the rotation values in the tbl.
        /// </remarks>
        void LaunchSpline(ushort splineId, SplineMode mode, float speed, bool rotation);

        /// <summary>
        /// Launch a new custom linear spline where the points are generated by <see cref="IMovementGenerator"/>.
        /// </summary>
        void LaunchGenerator(IMovementGenerator generator, float speed, SplineMode mode = SplineMode.OneShot);

        /// <summary>
        /// Launch a new follow spline, following the supplied <see cref="IWorldEntity"/> at distance.
        /// </summary>
        void Follow(IWorldEntity entity, float distance);
    }
}