using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0811)]
    public class Server0811 : IWritable
    {
        public uint CastingId { get; set; }
        public List<uint> CasterId { get; set; } = new List<uint>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(CasterId.Count, 32u);
            CasterId.ForEach(c => writer.Write(c));
        }
    }
}
