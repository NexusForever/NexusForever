using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
