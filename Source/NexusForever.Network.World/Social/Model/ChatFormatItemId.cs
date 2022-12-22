using NexusForever.Game.Static.Social;

namespace NexusForever.Network.World.Social.Model
{
    [ChatFormat(ChatFormatType.ItemItemId)]
    public class ChatFormatItemId : IChatFormat
    {
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
