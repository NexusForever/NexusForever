using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity.Movement.Command.Position
{
    public class PositionProjectileCommand : IPositionCommand
    {
        public EntityCommand Command => EntityCommand.SetPositionProjectile;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => timer?.HasElapsed ?? false;

        private uint flightTime;
        private float gravity;
        private Vector3 end;
        private Vector3 start;

        private UpdateTimer timer;

        /// <summary>
        /// Initialise command with the supplied flight time, gravity and positions.
        /// </summary>
        public void Initialise(uint flightTime, float gravity, Vector3 end, Vector3 start)
        {
            this.flightTime = flightTime;
            this.gravity    = gravity;
            this.end        = end;
            this.start      = start;

            timer = new UpdateTimer(TimeSpan.FromMilliseconds(flightTime));
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            timer.Update(lastTick);
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model = new SetPositionProjectileCommand
                {
                    End        = end,
                    Start      = start,
                    FlightTime = flightTime,
                    Gravity    = gravity
                }
            };
        }

        /// <summary>
        /// Returns the current <see cref="Vector3"/> position for the entity command.
        /// </summary>
        public Vector3 GetPosition()
        {
            // TODO: actually calculate this, just use the final position for now
            return end;
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
