using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerMapTrackedUnitDisable)]
    public class ServerMapTrackedUnitDisable : IWritable
    {
        public uint TrackedUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TrackedUnitId);
        }
    }
}
