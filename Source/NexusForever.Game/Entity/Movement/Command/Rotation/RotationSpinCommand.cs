using System.Numerics;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Rotation
{
    public class RotationSpinCommand : IRotationCommand
    {
        public EntityCommand Command => EntityCommand.SetRotationSpin;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised { get; private set; }

        private double flightTime;
        private float spin;
        private Vector3 rotation;

        private double duration;

        /// <summary>
        /// Initialise command with the specified rotation, duration and spin.
        /// </summary>
        public void Initialise(Vector3 rotation, TimeSpan duration, float spin)
        {
            flightTime           = duration.TotalSeconds;
            this.spin            = spin;
            this.rotation        = rotation;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            duration += lastTick;

            if (duration >= flightTime)
                IsFinalised = true;
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model   = new SetRotationSpinCommand
                {
                    Rotation   = rotation,
                    FlightTime = (uint)(flightTime * 1000d),
                    Spin       = spin,
                    Offset     = (uint)(duration * 1000d)
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 GetRotation()
        {
            float x = (((float)duration * spin) + rotation.X).NormaliseRotationRadians();
            return new Vector3(x, rotation.Y, rotation.Z);
        }
    }
}
