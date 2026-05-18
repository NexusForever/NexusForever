using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientItemUse)]
    public class ClientItemUse : IReadable
    {
        public uint CastingId { get; private set; }
        public ItemLocation Location { get; } = new();
        public uint TargetUnitId { get; private set; }
        public ItemLocation TargetLocation { get; } = new();
        public Position Position { get; } = new Position();

        public void Read(GamePacketReader reader)
        {
            CastingId = reader.ReadUInt();
            Location.Read(reader);
            TargetUnitId = reader.ReadUInt();
            TargetLocation.Read(reader);
            Position.Read(reader);
        }
    }
}
