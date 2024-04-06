using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;

namespace NexusForever.Game.Entity.Movement.Command.Position
{
    public class PositionCommand : IPositionCommand
    {
        public EntityCommand Command => EntityCommand.SetPosition;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private Vector3 position;
        private bool blend;

        /// <summary>
        /// Initialise command with the specified <see cref="Vector3"/> position and blend.
        /// </summary>
        public void Initialise(Vector3 position, bool blend)
        {
            this.position = position;
            this.blend    = blend;
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
                Model   = new SetPositionCommand
                {
                    Position = position,
                    Blend    = blend
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> position for the entity command.
        /// </summary>
        public Vector3 GetPosition()
        {
            return position;
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        /// <remarks>
        /// This is used to calculate the direction of travel for movement commands.
        /// </remarks>
        public Vector3 GetRotation()
        {
            return Vector3.Zero;
        }
    }
}
