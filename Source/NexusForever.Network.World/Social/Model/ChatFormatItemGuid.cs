using NexusForever.Game.Static.Social;

namespace NexusForever.Network.World.Social.Model
{
    [ChatFormat(ChatFormatType.ItemGuid)]
    public class ChatFormatItemGuid : IChatFormat
    {
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
