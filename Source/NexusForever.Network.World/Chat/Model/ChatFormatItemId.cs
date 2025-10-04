using NexusForever.Game.Static.Social;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatItemId : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemId;
        public uint ItemId { get; set; }

        public void Read(GamePacketReader reader)
        {
            ItemId = reader.ReadUInt(18u);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemId, 18u);
        }
    }
}
