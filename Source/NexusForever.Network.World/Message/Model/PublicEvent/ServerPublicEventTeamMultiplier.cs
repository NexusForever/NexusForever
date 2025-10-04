using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Used to scale the required amount for objectives of an event
    [Message(GameMessageOpcode.ServerPublicEventTeamMultiplier)]
    public class ServerPublicEventTeamMultiplier : IWritable
    {
        public uint EventId { get; set; }
        public uint TeamMultiplier { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventId, 14u);
            writer.Write(TeamMultiplier);
        }
    }
}
