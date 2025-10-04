using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Chat;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class ChatFormat : IWritable
    {
        public ChatFormatType Type { get; set; }
        public ushort StartIndex { get; set; }
        public ushort StopIndex { get; set; }
        public IChatFormatModel Model { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 4u);
            writer.Write(StartIndex);
            writer.Write(StopIndex);
            Model.Write(writer);
        }
    }
}
