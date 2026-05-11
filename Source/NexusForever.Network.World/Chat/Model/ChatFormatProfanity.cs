using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatProfanity : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Profanity;
        public uint RandomTextSeed { get; set; } // using current program time in milliseconds

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
