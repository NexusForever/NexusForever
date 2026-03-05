using NexusForever.Game.Static.Chat;

namespace NexusForever.Network.World.Chat.Model
{
    public class ChatFormatLoot : IChatFormatModel
    {
        public ChatFormatType Type => ChatFormatType.Loot;
        public uint LootUnitId { get; set; } // must match loot sent in ServerLootNotification (0x8A6)

        public void Read(GamePacketReader reader)
        {
            LootUnitId = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LootUnitId);
        }
    }
}
