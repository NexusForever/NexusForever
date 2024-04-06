using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity
{
    public class NetworkEntityCommand : INetworkEntityCommand
    {
        public EntityCommand Command { get; set; }
        public IEntityCommandModel Model { get; set; }
    }
}
