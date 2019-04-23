using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network
{
    public interface IEntityCommandModel
    {
        void Read(GamePacketReader reader);
        void Write(GamePacketWriter writer);
    }
}
