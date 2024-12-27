using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Velocity;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.Velocity
{
    public class VelocityCommandGroup : IVelocityCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => command.Command == EntityCommand.SetVelocityKeys;

        private IVelocityCommand command;

        #region Dependency Injection

        private readonly IFactoryInterface<IVelocityCommand> factory;

        public VelocityCommandGroup(
            IFactoryInterface<IVelocityCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IVelocityCommandGroup"/ with default command.
        /// </summary>
        public void Initialise()
        {
            SetVelocityDefaults();
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
            var command = factory.Resolve<VelocityCommand>();
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

            Vector3 velocity = GetVelocity();
            command = null;

            SetVelocity(velocity, false);
        }

        /// <summary>
        /// Get the current <see cref="Vector3"/> velocity value.
        /// </summary>
        public Vector3 GetVelocity()
        {
            return command.GetVelocity();
        }

        /// <summary>
        /// Set the velocity to the supplied <see cref="Vector3"/> value.
        /// </summary>
        public void SetVelocity(Vector3 velocity, bool blend)
        {
            Finalise();

            var command = factory.Resolve<VelocityCommand>();
            command.Initialise(velocity, blend);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set velocity to the interpolated <see cref="Vector3"/> velocity between the supplied times and velocities.
        /// </summary>
        public void SetVelocityKeys(List<uint> times, List<Vector3> modes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set velocity to the default values.
        /// </summary>
        public void SetVelocityDefaults()
        {
            Finalise();

            command = factory.Resolve<VelocityDefaultsCommand>();

            IsDirty = true;
        }
    }
}
