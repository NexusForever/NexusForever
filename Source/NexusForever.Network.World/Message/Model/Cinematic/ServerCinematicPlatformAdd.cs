using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicPlatformAdd)]
    public class ServerCinematicPlatformAdd : IWritable
    {
        public uint Delay { get; set; }
        public uint ActorUnitId { get; set; }
        public uint PlatformUnitId { get; set; }
        public uint AttachmentId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(ActorUnitId);
            writer.Write(PlatformUnitId);
            writer.Write(AttachmentId);
        }
    }
}
