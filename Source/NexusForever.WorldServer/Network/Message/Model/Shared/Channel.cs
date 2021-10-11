using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class Channel : IReadable, IWritable
    {
        public ChatChannelType Type { get; set; }
        public ulong ChatId { get; set; }

        public void Read(GamePacketReader reader)
        {
            Type   = reader.ReadEnum<ChatChannelType>(14u);
            ChatId = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 14u);
            writer.Write(ChatId);
        }
    }
}
