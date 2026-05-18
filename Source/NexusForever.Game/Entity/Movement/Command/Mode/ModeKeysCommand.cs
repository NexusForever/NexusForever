using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Mode;
using NexusForever.Game.Entity.Movement.Key;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Mode
{
    public class ModeKeysCommand : IModeCommand
    {
        public EntityCommand Command => EntityCommand.SetModeKeys;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => modeKeys?.IsFinalised ?? false;

        private ModeKeys modeKeys;

        /// <summary>
        /// Initialise command with the specified times and <see cref="ModeType"/> key values.
        /// </summary>
        public void Initialise(IMovementManager movementManager, List<uint> times, List<ModeType> modes)
        {
            modeKeys = new ModeKeys();
            modeKeys.Initialise(movementManager, times, modes);
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
                Model   = new SetModeKeysCommand
                {
                    Times = modeKeys.Times,
                    Modes = modeKeys.Values
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="ModeType"/> for the entity command.
        /// </summary>
        public ModeType GetMode()
        {
            return modeKeys.GetInterpolated();
        }
    }
}
