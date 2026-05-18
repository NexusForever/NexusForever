using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Scale;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command.Scale
{
    public class ScaleCommandGroup : IScaleCommandGroup
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        public bool RequiresSynchronisation => command.Command == EntityCommand.SetScaleKeys;

        private IScaleCommand command;

        private IMovementManager movementManager;

        #region Dependency Injection

        private readonly IFactoryInterface<IScaleCommand> factory;

        public ScaleCommandGroup(
            IFactoryInterface<IScaleCommand> factory)
        {
            this.factory = factory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IScaleCommandGroup"/> with default command.
        /// </summary>
        public void Initialise(IMovementManager movementManager)
        {
            this.movementManager = movementManager;

            SetScale(1f);
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
            var command = factory.Resolve<ScaleCommand>();
            command.Initialise(GetScale());
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

            float scale = GetScale();
            command = null;

            SetScale(scale);
        }

        /// <summary>
        /// Get the current scale value.
        /// </summary>
        public float GetScale()
        {
            return command.GetScale();
        }

        /// <summary>
        /// Set the scale to the supplied value.
        /// </summary>
        public void SetScale(float scale)
        {
            Finalise();

            var command = factory.Resolve<ScaleCommand>();
            command.Initialise(scale);
            this.command = command;

            IsDirty = true;
        }

        /// <summary>
        /// Set the scale to the interpolated value between supplied times and scales.
        /// </summary>
        public void SetScaleKeys(List<uint> times, List<float> scales)
        {
            Finalise();

            var command = factory.Resolve<ScaleKeysCommand>();
            command.Initialise(movementManager, times, scales);
            this.command = command;

            IsDirty = true;
        }
    }
}
