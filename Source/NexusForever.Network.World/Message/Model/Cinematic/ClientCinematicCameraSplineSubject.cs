using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ClientCinematicCameraSubjectSpline)]
    public class ClientCinematicCameraSplineSubject : IReadable
    {
        public uint SplineId { get; private set; }
        public uint SplineMode { get; private set; }
        public uint Speed { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            SplineId   = reader.ReadUInt();
            SplineMode = reader.ReadUInt();
            Speed   = reader.ReadUInt();
        }
    }
}
