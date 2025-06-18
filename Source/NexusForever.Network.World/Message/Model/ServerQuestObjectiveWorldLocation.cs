using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Sets the WorldLocation2Id for a specific quest objective
    [Message(GameMessageOpcode.ServerQuestObjectiveWorldLocation)]
    public class ServerQuestObjectiveWorldLocation : IWritable
    {
        public uint QuestId { get; set; }
        public uint QuestObjectiveIndex { get; set; }
        public uint WorldLocation2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(QuestId, 15u);
            writer.Write(QuestObjectiveIndex);
            writer.Write(WorldLocation2Id, 17u);
        }
    }
}
