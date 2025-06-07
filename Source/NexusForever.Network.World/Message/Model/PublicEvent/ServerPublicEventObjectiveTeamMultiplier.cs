using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventObjectiveTeamMultiplier)]
    public class ServerPublicEventObjectiveTeamMultiplier : IWritable
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
