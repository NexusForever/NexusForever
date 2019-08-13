using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Quest.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestStateChange)]
    public class ServerQuestStateChange : IWritable
    {
        public ushort QuestId { get; set; }
        public QuestState QuestState { get; set; }
        public uint RandomResultId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(QuestId, 15u);
            writer.Write(QuestState, 32u);
            writer.Write(RandomResultId);
        }
    }
}
