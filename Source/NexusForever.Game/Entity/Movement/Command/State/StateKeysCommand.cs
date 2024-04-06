using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.State;
using NexusForever.Game.Entity.Movement.Key;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.State;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.State
{
    internal class StateKeysCommand : IStateCommand
    {
        public EntityCommand Command => EntityCommand.SetStateKeys;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private StateKeys stateKeys;

        /// <summary>
        /// Initialise command with the specified times and <see cref="StateFlags"/> state key values.
        /// </summary>
        public void Initialise(IMovementManager movementManager, List<uint> times, List<StateFlags> states)
        {
            stateKeys = new StateKeys();
            stateKeys.Initialise(movementManager, times, states);
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
                Model   = new SetStateKeysCommand
                {
                    Times  = stateKeys.Times,
                    States = stateKeys.Values
                }
            };
        }

        /// <summary>
        /// Return the current <see cref="StateFlags"/> for the entity command.
        /// </summary>
        public StateFlags GetState()
        {
            return stateKeys.GetInterpolated();
        }
    }
}
