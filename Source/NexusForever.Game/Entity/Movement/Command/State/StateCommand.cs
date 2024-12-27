using NexusForever.Game.Abstract.Entity.Movement.Command.State;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.State;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.State
{
    public class StateCommand : IStateCommand
    {
        public EntityCommand Command => EntityCommand.SetState;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private StateFlags state;

        /// <summary>
        /// Initialise command with the specified <see cref="StateFlags"/>.
        /// </summary>
        public void Initialise(StateFlags state)
        {
            this.state = state;
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
                Model   = new SetStateCommand
                {
                    State = state
                }
            };
        }

        /// <summary>
        /// Return the current <see cref="StateFlags"/> for the entity command.
        /// </summary>
        public StateFlags GetState()
        {
            return state;
        }
    }
}
