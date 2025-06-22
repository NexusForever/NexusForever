using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingManager0x5B0)]
    public class ServerMatchingManager0x5B0 : IWritable
    {
        public bool Unknown { get; set; } // possibly indicates InMatchingGameGroup 

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown);
        }
    }
}
