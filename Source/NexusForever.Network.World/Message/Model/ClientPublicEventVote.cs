using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPublicEventVote)]
    public class ClientPublicEventVote : IReadable
    {
        public uint EventId { get; private set; }
        public uint VoteId { get; private set; }
        public uint TeamId { get; private set; }
        public uint Choice { get; private set; }

        public void Read(GamePacketReader reader)
        {
            EventId = reader.ReadUInt(14);
            VoteId  = reader.ReadUInt(14);
            TeamId  = reader.ReadUInt(14);
            Choice  = reader.ReadUInt();
        }
    }
}
