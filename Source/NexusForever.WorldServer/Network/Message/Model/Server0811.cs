using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0811)]
    public class Server0811 : IWritable
    {
        public uint CastingId { get; set; }
        public List<uint> SpellTargets { get; set; } = new List<uint>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(SpellTargets.Count, 32u);
            SpellTargets.ForEach(c => writer.Write(c));
        }
    }
}
