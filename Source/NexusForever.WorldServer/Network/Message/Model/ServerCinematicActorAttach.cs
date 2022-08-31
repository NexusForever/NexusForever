using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicActorAttach)]
    public class ServerCinematicCameraAttach : IWritable
    {
        public uint AttachType { get; set; }
        public uint AttachId { get; set; }
        public uint Delay { get; set; }
        public uint ParentUnit { get; set; }
        public bool UseRotation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AttachType);
            writer.Write(AttachId);
            writer.Write(Delay);
            writer.Write(ParentUnit);
            writer.Write(UseRotation);
        }
    }
}
