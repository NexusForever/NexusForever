using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Game.Social.Model
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
