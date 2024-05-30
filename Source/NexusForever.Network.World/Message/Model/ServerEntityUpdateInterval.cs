using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerClientEntityUpdateInterval)]
    public class ServerClientEntityUpdateInterval : IWritable
    {
        public uint ClientEntityMaxSendUpdateInterval { get; set; }
        // Interval is specified in milliseconds.
        // Client will bound the value received between 25 and 1000 ms.
        // Captured data show that the server would periodically change the interval between 125 and 126 ms
        // but the reasoning for this is unclear.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ClientEntityMaxSendUpdateInterval);
        }
    }
}
