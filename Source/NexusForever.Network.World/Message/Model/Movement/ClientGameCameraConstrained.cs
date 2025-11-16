using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model
{
    // Seems like this is sent when the game camera is constrained by the world geometry to the player unit's bounding box
    // TODO: research more
    [Message(GameMessageOpcode.ClientGameCameraConstrained)]
    public class ClientGameCameraConstrained : IReadable
    {
        public bool Unknown { get; private set; }
        public Vector3 Position { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown = reader.ReadBit();
            Position = reader.ReadVector3();
        }
    }
}
