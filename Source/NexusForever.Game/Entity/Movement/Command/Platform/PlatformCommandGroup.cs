using NexusForever.Game.Abstract.Entity.Movement.Command.Platform;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.Platform
{
    public class PlatformCommandGroup : IPlatformCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => false;

        private IPlatformCommand command;

        #region Dependency Injection

        private readonly IFactoryInterface<IPlatformCommand> factory;

        public PlatformCommandGroup(
            IFactoryInterface<IPlatformCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IPlatformCommandGroup"/ with default command.
        /// </summary>
        public void Initialise()
        {
            SetPlatform(null);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            // deliberately empty
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
        /// Get the current platform unit id value.
        /// </summary>
        public uint? GetPlatform()
        {
            return command.GetPlatform();
        }

        /// <summary>
        /// Set the platform to the supplied platform unit id value.
        /// </summary>
        public void SetPlatform(uint? platformUnitId)
        {
            var command = factory.Resolve<PlatformCommand>();
            command.Initialise(platformUnitId);
            this.command = command;

            IsDirty = true;
        }
    }
}
