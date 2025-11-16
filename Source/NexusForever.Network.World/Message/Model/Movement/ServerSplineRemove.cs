using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Movement
{
    // Removes spline from the live spline set
    [Message(GameMessageOpcode.ServerSplineRemove)]
    public class ServerSplineRemove : IWritable
    {
        public uint Spline2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spline2Id);
        }
    }
}
