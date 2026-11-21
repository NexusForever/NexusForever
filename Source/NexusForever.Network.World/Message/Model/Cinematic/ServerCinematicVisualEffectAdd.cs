using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicVisualEffectAdd)]
    public class ServerCinematicVisualEffectAdd : IWritable
    {
        public uint Delay { get; set; }
        public uint VisualEffectUniqueId { get; set; }
        public uint VisualEffectId { get; set; }
        public uint UnitId { get; set; }
        public Position Position { get; set; }
        public bool RemoveOnCameraEnd { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(VisualEffectUniqueId);
            writer.Write(VisualEffectId, 17u);
            writer.Write(UnitId);
            Position.Write(writer);
            writer.Write(RemoveOnCameraEnd);
        }
    }
}
