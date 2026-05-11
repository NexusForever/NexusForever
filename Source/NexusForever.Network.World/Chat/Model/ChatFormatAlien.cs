using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatAlien : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Alien;
        public uint RandomTextSeed { get; set; }

        public void Read(GamePacketReader reader)
        {
            RandomTextSeed = reader.ReadUShort(14u);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RandomTextSeed);
        }
    }
}
