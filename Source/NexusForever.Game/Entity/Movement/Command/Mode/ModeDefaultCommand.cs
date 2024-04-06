using NexusForever.Game.Abstract.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Mode
{
    public class ModeDefaultCommand : IModeCommand
    {
        public EntityCommand Command => EntityCommand.SetModeDefault;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised { get; }

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
                Model   = new SetModeDefaultCommand
                {
                }
            };
        }

        /// <summary>
        /// Get the current <see cref="ModeType"/> value.
        /// </summary>
        public ModeType GetMode()
        {
            return ModeType.Walk;
        }
    }
}
