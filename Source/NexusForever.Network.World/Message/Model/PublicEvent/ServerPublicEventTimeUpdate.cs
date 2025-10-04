using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventTimeUpdate)]
    public class ServerPublicEventTimeUpdate : IWritable
    {
        public uint EventId { get; set; }
        public bool Busy { get; set; }
        public uint ElapsedTimeMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventId, 14u);
            writer.Write(Busy);
            writer.Write(ElapsedTimeMs);
        }
    }
}
