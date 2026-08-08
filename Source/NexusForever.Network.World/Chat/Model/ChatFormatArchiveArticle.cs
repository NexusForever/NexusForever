using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatArchiveArticle : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ArchiveArticle;
        public ushort ArchiveArticleId { get; set; }

        public void Read(GamePacketReader reader)
        {
            ArchiveArticleId = reader.ReadUShort(14u);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ArchiveArticleId, 14u);
        }
    }
}
