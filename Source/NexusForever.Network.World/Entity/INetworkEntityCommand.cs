using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity
{
    public interface INetworkEntityCommand
    {
        EntityCommand Command { get; set; }
        IEntityCommandModel Model { get; set; }
    }
}