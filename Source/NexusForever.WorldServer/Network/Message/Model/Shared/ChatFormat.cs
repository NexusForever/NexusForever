using System;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
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

            FormatModel = SocialManager.Instance.GetChatFormatModel(Type);
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
