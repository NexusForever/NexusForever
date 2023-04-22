using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestObjectiveUpdate)]
    public class ServerQuestObjectiveUpdate : IWritable
    {
        public ushort QuestId { get; set; }
        public uint Index { get; set; }
        public uint Completed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(QuestId, 15u);
            writer.Write(Index);
            writer.Write(Completed);
        }
    }
}
