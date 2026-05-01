using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatItemId : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemId;
        public uint Item2Id { get; set; }

        public void Read(GamePacketReader reader)
        {
            Item2Id = reader.ReadUInt(18u);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Item2Id, 18u);
        }
    }
}
