using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityVisualEffect)]
    public class ServerEntityVisualEffect : IWritable
    {
        public uint ServerUniqueId { get; set; }
        public uint SourceUnitId { get; set; }
        public uint VisualEffectId { get; set; } // 17u
        public uint TimeElapsed { get; set; } // 17u
        public uint Unknown0 { get; set; } // 17u
        public uint Unknown1 { get; set; }
        public Position SourceLocation { get; set; } = new Position();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(SourceUnitId);
            writer.Write(VisualEffectId, 17u);
            writer.Write(TimeElapsed, 17u);
            writer.Write(Unknown0, 17u);
            writer.Write(Unknown1);
            SourceLocation.Write(writer);
        }
    }
}
