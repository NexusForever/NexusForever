using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Triggers a repeating event for BonusEventsChanged
    [Message(GameMessageOpcode.ServerPublicEventTriggerUiUpdates)]
    public class ServerPublicEventTriggerUiUpdates : IWritable
    {
        public uint EventId { get; set; }
        public bool Start { get; set; } // true = triggers PublicEventStart, false = triggers PublicEventCleared

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventId, 14);
            writer.Write(Start);
        }
    }
}
