using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicVisualEffect)]
    public class ServerCinematicVisualEffect : IWritable
    {
        public uint Delay { get; set; }
        public uint VisualHandle { get; set; }
        public uint VisualEffectId { get; set; } // 17
        public uint UnitId { get; set; }
        public Position Position { get; set; }
        public bool RemoveOnCameraEnd { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(VisualHandle);
            writer.Write(VisualEffectId, 17u);
            writer.Write(UnitId);
            Position.Write(writer);
            writer.Write(RemoveOnCameraEnd);
        }
    }
}
