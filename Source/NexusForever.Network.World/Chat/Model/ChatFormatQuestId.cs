using NexusForever.Game.Static.Social;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatQuestId : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.QuestId;
        public ushort QuestId { get; set; }

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUShort(15u);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(QuestId, 15u);
        }
    }
}
