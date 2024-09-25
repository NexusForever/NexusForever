using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPublicEventVoteEnd)]
    public class ServerPublicEventVoteEnd : IWritable
    {
        public uint EventId { get; set; }
        public uint VoteId { get; set; }
        public uint Winner { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventId, 14);
            writer.Write(VoteId, 14);
            writer.Write(Winner);
        }
    }
}
