using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemUse, MessageDirection.Client)]
    public class ClientItemUse : IReadable
    {
        public uint CastingId { get; private set; }
        public ItemLocation Location { get; } = new ItemLocation();
        public uint TargetUnitId { get; private set; }
        public ItemLocation TargetLocation { get; } = new ItemLocation();
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
