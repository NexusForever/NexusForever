using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathUpdateXP)]
    public class ServerPathUpdateXP : IWritable
    {
        public uint TotalXP { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TotalXP);
        }
    }
}
