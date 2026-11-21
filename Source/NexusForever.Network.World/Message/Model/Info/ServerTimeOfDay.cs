using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Info
{
    [Message(GameMessageOpcode.ServerTimeOfDay)]
    public class ServerTimeOfDay : IWritable
    {
        public uint TimeOfDay { get; set; } // in seconds
        public uint Season { get; set; }
        public uint LengthOfDay { get; set; } // in seconds

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TimeOfDay);
            writer.Write(Season);
            writer.Write(LengthOfDay);
        }
    }
}
