using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Social;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class ChatFormat : IReadable, IWritable
    {
        public ChatFormatType Type { get; set; }
        public ushort StartIndex { get; set; }
        public ushort StopIndex { get; set; }
        public IChatFormat FormatModel { get; set; }

        public void Read(GamePacketReader reader)
        {
            Type        = reader.ReadEnum<ChatFormatType>(4);
            StartIndex  = reader.ReadUShort();
            StopIndex   = reader.ReadUShort();

            FormatModel = ChatFormatManager.Instance.NewChatFormatModel(Type);
            if (FormatModel == null)
                throw new NotImplementedException();

            FormatModel.Read(reader);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 4u);
            writer.Write(StartIndex);
            writer.Write(StopIndex);
            FormatModel.Write(writer);
        }
    }
}
