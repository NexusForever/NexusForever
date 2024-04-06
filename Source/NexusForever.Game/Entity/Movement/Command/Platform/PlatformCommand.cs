using NexusForever.Game.Abstract.Entity.Movement.Command.Platform;
using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Command;

namespace NexusForever.Game.Entity.Movement.Command.Platform
{
    public class PlatformCommand : IPlatformCommand
    {
        public EntityCommand Command => EntityCommand.SetPlatform;

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        public bool IsFinalised => false;

        private uint? platformUnitId;

        /// <summary>
        /// Initialise command with the specified platform unit id.
        /// </summary>
        public void Initialise(uint? platformUnitId)
        {
            this.platformUnitId = platformUnitId;
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
                Model   = new SetPlatformCommand
                {
                    UnitId = platformUnitId ?? 0u
                }
            };
        }

        /// <summary>
        /// Returns the current platform unit id for the entity command.
        /// </summary>
        public uint? GetPlatform()
        {
            return platformUnitId;
        }
    }
}
