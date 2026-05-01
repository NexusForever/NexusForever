using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatNavPoint : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.NavPoint;
        public ushort MapZoneId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public void Read(GamePacketReader reader)
        {
            MapZoneId = reader.ReadUShort(14u);
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MapZoneId, 14u);
            writer.Write(X);
            writer.Write(Y);
        }
    }
}
