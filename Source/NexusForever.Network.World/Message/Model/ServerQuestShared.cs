using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestShared)]
    public class ServerQuestShared : IWritable
    {
        public uint SharerUnitId { get; set; }
        public uint QuestId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SharerUnitId);
            writer.Write(QuestId, 15u);
        }
    }
}
