using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerTimeOfDay)]
    public class ServerTimeOfDay : IWritable
    {
        public uint TimeOfDay { get; set; }
        public uint Season { get; set; }
        public uint LengthOfDay { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TimeOfDay);
            writer.Write(Season);
            writer.Write(LengthOfDay);
        }
    }
}
