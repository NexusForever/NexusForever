using NexusForever.Game.Abstract.Entity.Movement.Command.Time;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity.Command;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Entity.Movement.Command.Time
{
    public class TimeCommand : ITimeCommand
    {
        public EntityCommand Command => EntityCommand.SetTime;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private double serverTime;

        /// <summary>
        /// Initialise command with the specified <see cref="TimeSpan"/.
        /// </summary>
        public void Initialise(TimeSpan timeSpan)
        {
            serverTime = timeSpan.TotalSeconds;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            serverTime += lastTick;
        }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        public INetworkEntityCommand GetNetworkEntityCommand()
        {
            return new NetworkEntityCommand
            {
                Command = Command,
                Model   = new SetTimeCommand
                {
                    Time = GetTime()
                }
            };
        }

        /// <summary>
        /// Return the current time in milliseconds for the entity command.
        /// </summary>
        public uint GetTime()
        {
            return (uint)(serverTime * 1000d);
        }
    }
}
