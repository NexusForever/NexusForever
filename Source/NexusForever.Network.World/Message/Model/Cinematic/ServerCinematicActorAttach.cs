using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicCameraAttach)]
    public class ServerCinematicCameraAttach : IWritable
    {
        public uint AttachType { get; set; }
        public uint AttachId { get; set; }
        public uint Delay { get; set; }
        public uint ParentUnitId { get; set; }
        public bool UseRotation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AttachType);
            writer.Write(AttachId);
            writer.Write(Delay);
            writer.Write(ParentUnitId);
            writer.Write(UseRotation);
        }
    }
}
