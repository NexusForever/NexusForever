using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ClientCinematicCameraSubjectPosVel)]
    public class ClientCinematicCameraSubjectPosVel : IReadable
    {
        public Position Position { get; private set; }
        public Vector3 Velocity { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Position.Read(reader);
            Velocity = new Vector3
            (
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
        }
    }
}
