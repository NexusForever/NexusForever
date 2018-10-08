using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network
{
    public interface IEntityCommand
    {
        void Read(GamePacketReader reader);
        void Write(GamePacketWriter writer);
    }
}
