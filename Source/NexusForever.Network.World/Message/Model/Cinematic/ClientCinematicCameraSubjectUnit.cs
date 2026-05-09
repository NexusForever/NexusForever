using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ClientCinematicCameraSubjectUnit)]
    public class ClientCinematicCameraSubjectUnit : IReadable
    {
        public uint UnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
        }
    }
}
