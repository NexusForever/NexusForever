using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Game.Social.Model
{
    [ChatFormat(ChatFormatType.QuestId)]
    public class ChatFormatQuestId : IChatFormat
    {
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
