using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatQuestId : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.QuestId;
        public ushort Quest2Id { get; set; }

        public void Read(GamePacketReader reader)
        {
            Quest2Id = reader.ReadUShort(15u);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Quest2Id, 15u);
        }
    }
}
