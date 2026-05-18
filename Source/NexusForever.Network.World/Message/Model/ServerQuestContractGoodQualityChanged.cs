using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestContractGoodQualityChanged)]
    public class ServerQuestContractGoodQualityChanged : IWritable
    {
        public uint[] QuestIds { get; set; } = new uint[3];

        public void Write(GamePacketWriter writer)
        {
            foreach (var id in QuestIds)
            {
                writer.Write(id);
            }
        }
    }
}
