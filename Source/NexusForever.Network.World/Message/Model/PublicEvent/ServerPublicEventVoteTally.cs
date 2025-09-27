using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Fires whenever a group member selects an option during a public event vote.
    [Message(GameMessageOpcode.ServerPublicEventVoteTally)]
    public class ServerPublicEventVoteTally : IWritable
    {
        public uint EventId { get; set; }
        public uint VoteId { get; set; }
        public uint Choice { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventId, 14);
            writer.Write(VoteId, 14);
            writer.Write(Choice);
        }
    }
}
