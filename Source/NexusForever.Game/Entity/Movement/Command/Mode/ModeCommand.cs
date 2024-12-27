using NexusForever.Game.Abstract.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Mode
{
    public class ModeCommand : IModeCommand
    {
        public EntityCommand Command => EntityCommand.SetMode;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private ModeType mode;

        /// <summary>
        /// Initialise command with the specified <see cref="ModeType"/>.
        /// </summary>
        public void Initialise(ModeType mode)
        {
            this.mode = mode;
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
                Model   = new SetModeCommand
                {
                    Mode = mode
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="ModeType"/> for the entity command.
        /// </summary>
        public ModeType GetMode()
        {
            return mode;
        }
    }
}
