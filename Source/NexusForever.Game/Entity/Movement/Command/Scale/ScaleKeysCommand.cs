using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Scale;
using NexusForever.Game.Entity.Movement.Key;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Scale
{
    public class ScaleKeysCommand : IScaleCommand
    {
        public EntityCommand Command => EntityCommand.SetScaleKeys;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => scaleKeys?.IsFinalised ?? false;

        private ScaleKeys scaleKeys;

        /// <summary>
        /// Initialise command with the specified times and scale key values.
        /// </summary>
        public void Initialise(IMovementManager movementManager, List<uint> times, List<float> scales)
        {
            scaleKeys = new ScaleKeys();
            scaleKeys.Initialise(movementManager, times, scales);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            // deliberately empty
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model   = new SetScaleKeysCommand
                {
                    Times  = scaleKeys.Times,
                    Scales = scaleKeys.Values
                }
            };
        }

        /// <summary>
        /// Return the current scale for the entity command.
        /// </summary>
        public float GetScale()
        {
            return scaleKeys.GetInterpolated();
        }
    }
}
