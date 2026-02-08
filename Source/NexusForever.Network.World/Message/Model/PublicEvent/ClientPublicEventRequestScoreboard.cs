using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Toggles whether the PublicEventLiveStatsUpdate event will fire for this public event.
    [Message(GameMessageOpcode.ClientPublicEventRequestScoreboard)]
    public class ClientPublicEventRequestScoreboard : IReadable
    {
        public uint EventId { get; private set; }
        public bool Subscribe { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            EventId = reader.ReadUInt(14u);
            Subscribe = reader.ReadBit();
        }
    }
}
