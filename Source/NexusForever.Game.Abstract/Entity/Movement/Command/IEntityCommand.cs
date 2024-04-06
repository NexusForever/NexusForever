using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity.Movement.Command
{
    public interface IEntityCommand : IUpdate
    {
        EntityCommand Command { get; }

        /// <summary>
        /// Returns if the command has been finalised.
        /// </summary>
        bool IsFinalised { get; }

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command.
        /// </summary>
        INetworkEntityCommand GetNetworkEntityCommand();
    }
}
