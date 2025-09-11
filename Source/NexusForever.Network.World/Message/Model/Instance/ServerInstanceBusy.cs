using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Prevents resurrection interaction and causes InstanceSetBusy event
    [Message(GameMessageOpcode.ServerInstanceBusy)]
    public class ServerInstanceBusy : IWritable
    {
        public bool Busy { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Busy);
        }
    }
}
