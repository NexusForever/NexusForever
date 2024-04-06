using NexusForever.Game.Abstract.Entity.Movement.Command.Scale;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Scale
{
    public class ScaleCommand : IScaleCommand
    {
        public EntityCommand Command => EntityCommand.SetScale;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private float scale;

        /// <summary>
        /// Initialise command with the specified scale.
        /// </summary>
        public void Initialise(float scale)
        {
            this.scale = scale;
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
                Model   = new SetScaleCommand
                {
                    Scale = scale
                }
            };
        }

        /// <summary>
        /// Return the current scale for the entity command.
        /// </summary>
        public float GetScale()
        {
            return scale;
        }
    }
}
