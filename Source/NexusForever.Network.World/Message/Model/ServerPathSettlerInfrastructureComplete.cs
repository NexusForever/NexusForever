using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerInfrastructureComplete)]
    public class ServerPathSettlerInfrastructureComplete : IWritable
    {
        // Client unpacks this value but the handling code does nothing with it.
        public uint Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused);
        }
    }
}
