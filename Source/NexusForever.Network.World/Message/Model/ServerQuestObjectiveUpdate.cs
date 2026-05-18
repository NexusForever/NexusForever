using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestObjectiveUpdate)]
    public class ServerQuestObjectiveUpdate : IWritable
    {
        public ushort QuestId { get; set; }
        public uint QuestObjectiveIndex { get; set; } // Not to be confused with QuestObjectiveId for tbl records
        public uint Completed { get; set; } // Objective completion flags, can represent different types of data i.e. not always flags, sometimes a number

        public void Write(GamePacketWriter writer)
        {
            writer.Write(QuestId, 15u);
            writer.Write(QuestObjectiveIndex);
            writer.Write(Completed);
        }
    }
}
