using System.Numerics;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Map
{
    [Message(GameMessageOpcode.ServerDestinationArrowSet)]
    public class ServerDestinationArrowSet : IWritable
    {
        public Vector3 DestinationPosition { get; set; }
        public float Radius { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteVector3(DestinationPosition);
            writer.Write(Radius);
        }
    }
}
