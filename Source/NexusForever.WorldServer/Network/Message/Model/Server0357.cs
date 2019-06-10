using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0357)]
    public class Server0357 : IWritable
    {
        public uint UnitId { get; set; }
        public bool Unknown { get; set; } // not used in packet handler

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Unknown);
        }
    }
}
