using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network
{
    public interface IEntityModel
    {
        void Write(GamePacketWriter writer);
    }
}
