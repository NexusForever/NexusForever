using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Entity.Movement.Key;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Position
{
    public class PositionKeysCommand : IPositionCommand
    {
        public EntityCommand Command => EntityCommand.SetPositionKeys;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => positionKeys?.IsFinalised ?? false;

        private readonly PositionKeys positionKeys = new();

        /// <summary>
        /// Initialise command with the specified times and <see cref="Vector3"/> position key values.
        /// </summary>
        public void Initialise(IMovementManager movementManager, List<uint> times, List<Vector3> positions)
        {
            positionKeys.Initialise(movementManager, times, positions);
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
                Model   = new SetPositionKeysCommand
                {
                    Times     = positionKeys.Times,
                    Positions = positionKeys.Values
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> position for the entity command.
        /// </summary>
        public Vector3 GetPosition()
        {
            return positionKeys.GetInterpolated();
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> rotation for the entity command.
        /// </summary>
        /// <remarks>
        /// This is used to calculate the direction of travel for movement commands.
        /// </remarks>
        public Vector3 GetRotation()
        {
            return positionKeys.GetRotation();
        }
    }
}
