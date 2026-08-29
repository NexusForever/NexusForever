using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    // Only used to trigger a UI event. Not seen in sniffs but perfectly usable
    [Message(GameMessageOpcode.ServerPublicEventObjectiveStart)]
    public class ServerPublicEventObjectiveStart : IWritable
    {
        public uint ObjectiveId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ObjectiveId, 15);
        }
    }
}
