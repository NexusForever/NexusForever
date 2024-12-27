using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.State;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Command.State;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.State
{
    public class StateDefaultCommand : IStateCommand
    {
        public EntityCommand Command => EntityCommand.SetStateDefault;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised { get; }

        private IMovementManager movementManager;

        /// <summary>
        /// Initialise command.
        /// </summary>
        public void Initialise(IMovementManager movementManager)
        {
            this.movementManager = movementManager;
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
                Model   = new SetStateDefaultCommand
                {
                }
            };
        }

        /// <summary>
        /// Return the current <see cref="StateFlags"/> for the entity command.
        /// </summary>
        public StateFlags GetState()
        {
            StateFlags flags = StateFlags.None;

            if (movementManager.GetVelocity().Length() > 0f)
                flags |= StateFlags.Velocity;

            if (movementManager.GetMove().Length() > 0f)
                flags |= StateFlags.Move;

            switch (movementManager.GetMode())
            {
                case ModeType.Walk:
                    flags |= StateFlags.Unknown100;
                    break;
                case ModeType.Swim:
                    flags |= StateFlags.Unknown80 | StateFlags.Unknown200;
                    break;
                case ModeType.Slide:
                    flags |= StateFlags.Unknown80 | StateFlags.Unknown400;
                    break;
                case ModeType.Free:
                    flags |= StateFlags.Unknown80;
                    break;
            }

            // TODO: client does more, research this

            return flags;
        }
    }
}
