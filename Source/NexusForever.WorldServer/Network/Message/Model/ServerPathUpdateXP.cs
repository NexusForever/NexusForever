using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
