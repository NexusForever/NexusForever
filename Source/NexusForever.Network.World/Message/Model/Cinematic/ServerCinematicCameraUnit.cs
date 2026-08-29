using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicCameraSubject)]
    public class ServerCinematicCameraUnit : IWritable
    {
        public uint Delay { get; set; }
        public uint UnitId { get; set; }
        public bool Target { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(UnitId);
            writer.Write(Target);
        }
    }
}
