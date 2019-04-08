using System;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerPlayed)]
    public class ServerPlayerPlayed : IWritable
    {
        public DateTime CreateTime { get; set; }
        public uint  TimePlayedSession { get; set; }
        public uint  TimePlayedTotal { get; set; }
        public uint  TimePlayedLevel { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write((ulong)CreateTime.ToFileTime());
            writer.Write(TimePlayedSession);
            writer.Write(TimePlayedTotal);
            writer.Write(TimePlayedLevel);
        }
    }
}
