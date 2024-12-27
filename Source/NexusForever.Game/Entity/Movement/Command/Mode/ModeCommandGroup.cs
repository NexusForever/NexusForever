using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.Mode
{
    public class ModeCommandGroup : IModeCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => command.Command == EntityCommand.SetModeKeys;

        private IModeCommand command;

        private IMovementManager movementManager;

        #region Dependency Injection

        private readonly IFactoryInterface<IModeCommand> factory;

        public ModeCommandGroup(
            IFactoryInterface<IModeCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IModeCommandGroup"/ with default command.
        /// </summary>
        public void Initialise(IMovementManager movementManager)
        {
            this.movementManager = movementManager;

            SetModeDefault();
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
            var command = factory.Resolve<ModeCommand>();
            command.Initialise(ModeType.Walk);
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

            ModeType mode = GetMode();
            command = null;

            SetMode(mode);
        }

        /// <summary>
        /// Get the current <see cref="ModeType"/> value.
        /// </summary>
        public ModeType GetMode()
        {
            return command.GetMode();
        }

        /// <summary>
        /// Set the mode to the supplied <see cref="ModeType"/> value.
        /// </summary>
        public void SetMode(ModeType mode)
        {
            Finalise();

            var command = factory.Resolve<ModeCommand>();
            command.Initialise(mode);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set mode to the interpolated <see cref="ModeType"/> between the supplied times and modes.
        /// </summary>
        public void SetModeKeys(List<uint> times, List<ModeType> modes)
        {
            Finalise();

            var command = factory.Resolve<ModeKeysCommand>();
            command.Initialise(movementManager, times, modes);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set mode to the default <see cref="ModeType"/> value.
        /// </summary>
        public void SetModeDefault()
        {
            Finalise();

            command = factory.Resolve<ModeDefaultCommand>();

            IsDirty = true;
        }
    }
}
