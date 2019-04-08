using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0908)]
    public class Server0908 : IWritable
    {
        public uint CasterId { get; set; }
        public uint TargetId { get; set; }
        public uint Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterId);
            writer.Write(TargetId);
            writer.Write(Unknown0);
        }
    }
}
