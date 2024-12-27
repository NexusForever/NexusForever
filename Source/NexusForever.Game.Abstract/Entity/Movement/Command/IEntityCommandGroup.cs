using NexusForever.Network.World.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity.Movement.Command
{
    public interface IEntityCommandGroup : IUpdate
    {
        /// <summary>
        /// Determines if the group has been modified and needs to be sent to the client.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Determines if the group has an entity command that requires resynchronisation after loading.
        /// </summary>
        bool RequiresSynchronisation { get; }

        /// <summary>
        /// 
        /// </summary>
        INetworkEntityCommand GetDefaultNetworkEntityCommand();

        /// <summary>
        /// Returns the <see cref="INetworkEntityCommand"/> for the entity command group.
        /// </summary>
        INetworkEntityCommand GetNetworkEntityCommand();

        /// <summary>
        /// Finalise the current entity command.
        /// </summary>
        void Finalise();
    }
}
