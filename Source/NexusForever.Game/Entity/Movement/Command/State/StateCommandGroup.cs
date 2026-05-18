using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.State;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.State;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.State
{
    public class StateCommandGroup : IStateCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => command.Command == EntityCommand.SetStateKeys;

        private IStateCommand command;

        private IMovementManager movementManager;

        #region Dependency Injection

        private readonly IFactoryInterface<IStateCommand> factory;

        public StateCommandGroup(
            IFactoryInterface<IStateCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IStateCommandGroup"/> with default command.
        /// </summary>
        public void Initialise(IMovementManager movementManager)
        {
            this.movementManager = movementManager;
            
            SetStateDefault();
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
            var command = factory.Resolve<StateCommand>();
            command.Initialise(StateFlags.None);
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

            StateFlags state = GetState();
            command = null;

            SetState(state);
        }

        /// <summary>
        /// Get the current <see cref="StateFlags"/> value.
        /// </summary>
        public StateFlags GetState()
        {
            return command.GetState();
        }

        /// <summary>
        /// Set the state flags to the supplied <see cref="StateFlags"/> value.
        /// </summary>
        public void SetState(StateFlags state)
        {
            Finalise();

            var command = factory.Resolve<StateCommand>();
            command.Initialise(state);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set the state to the interpolated <see cref="StateFlags"/> between the supplied times and stage flags.
        /// </summary>
        public void SetStateKeys(List<uint> times, List<StateFlags> states)
        {
            Finalise();

            var command = factory.Resolve<StateKeysCommand>();
            command.Initialise(movementManager, times, states);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set the default <see cref="StateFlags"/> value.
        /// </summary>
        public void SetStateDefault()
        {
            Finalise();

            var command = factory.Resolve<StateDefaultCommand>();
            command.Initialise(movementManager);
            this.command = command;

            IsDirty = true;
        }
    }
}
