using NexusForever.Game.Abstract.Entity.Movement.Command.Time;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.Time
{
    public class TimeCommandGroup : ITimeCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => false;

        /// <summary>
        /// Determines if server time has been reset.
        /// </summary>
        public bool TimeReset { get; set; }

        private ITimeCommand command;

        #region Dependency Injection

        private readonly IFactoryInterface<ITimeCommand> factory;

        public TimeCommandGroup(
            IFactoryInterface<ITimeCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="ITimeCommandGroup"/> with default command.
        /// </summary>
        public void Initialise()
        {
            SetTime(TimeSpan.FromSeconds(0d));
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            command.Update(lastTick);
        }

        /// <summary>
        /// Return the default <see cref="INetworkEntityCommand"/> for the entity command group.
        /// </summary>
        /// <remarks>
        /// The default command to send if the entity requires synchronisation.
        /// </remarks>
        public INetworkEntityCommand GetDefaultNetworkEntityCommand()
        {
            return GetNetworkEntityCommand();
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
            // deliberately empty
        }

        /// <summary>
        /// Get the current server time.
        /// </summary>
        public uint GetTime()
        {
            return command.GetTime();
        }

        /// <summary>
        /// Set the time to the specified <see cref="TimeSpan"/>.
        /// </summary>
        public void SetTime(TimeSpan timeSpan)
        {
            var command = factory.Resolve<TimeCommand>();
            command.Initialise(timeSpan);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Reset the current server time to 0.
        /// </summary>
        /// <remarks>
        /// This will reset the time synchronisation information at the client.
        /// </remarks>
        public void ResetTime()
        {
            SetTime(TimeSpan.FromSeconds(0d));
            TimeReset = true;
        }
    }
}
