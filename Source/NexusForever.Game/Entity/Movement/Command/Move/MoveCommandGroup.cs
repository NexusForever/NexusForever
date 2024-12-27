using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Move;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.Move
{
    public class MoveCommandGroup : IMoveCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => command.Command == EntityCommand.SetModeKeys;

        private IMoveCommand command;

        private IMovementManager movementManager;

        #region Dependency Injection

        private readonly IFactoryInterface<IMoveCommand> factory;

        public MoveCommandGroup(
            IFactoryInterface<IMoveCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMoveCommandGroup"/ with default command.
        /// </summary>
        public void Initialise(IMovementManager movementManager)
        {
            this.movementManager = movementManager;

            SetMoveDefaults(false);
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
            var command = factory.Resolve<MoveCommand>();
            command.Initialise(Vector3.Zero, false);
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

            Vector3 move = GetMove();
            command = null;

            SetMove(move, false);
        }

        /// <summary>
        /// Get the current <see cref="Vector3"/> move value.
        /// </summary>
        public Vector3 GetMove()
        {
            return command.GetMove();
        }

        /// <summary>
        /// Set the mode to the supplied <see cref="Vector3"/> move value.
        /// </summary>
        public void SetMove(Vector3 move, bool blend)
        {
            Finalise();

            var command = factory.Resolve<MoveCommand>();
            command.Initialise(move, blend);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set mode to the interpolated <see cref="Vector3"/> between the supplied times and modes.
        /// </summary>
        public void SetMoveKeys(List<uint> times, List<Vector3> moves)
        {
            Finalise();

            var command = factory.Resolve<MoveKeysCommand>();
            command.Initialise(movementManager, times, moves);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set mode to the default <see cref="Vector3"/> move value.
        /// </summary>
        public void SetMoveDefaults(bool blend)
        {
            Finalise();

            var command = factory.Resolve<MoveDefaultsCommand>();
            command.Initialise(blend);
            this.command = command;

            IsDirty = true;
        }
    }
}
