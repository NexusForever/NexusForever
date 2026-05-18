using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatItemGuid : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemGuid;
        public ulong ItemGuid { get; set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
        }
    }
}
