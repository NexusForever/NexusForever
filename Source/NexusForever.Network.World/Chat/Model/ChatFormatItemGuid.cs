using NexusForever.Game.Static.Social;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatItemGuid : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.ItemGuid;
        public ulong Guid { get; set; }

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
        }
    }
}
