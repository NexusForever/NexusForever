using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildBankTransaction2)]
    public class ClientGuildBankTransaction2 : IReadable
    {
        public Identity GuildIdentity { get; set; } = new Identity();
        public ulong ItemGuidFrom { get; set; }
        public ulong ItemGuidTo { get; set; } // Sent if bag slot already contains an item, otherwise 0
        public uint Item2IdFrom { get; set; }
        public ItemLocation LocationTo { get; set; } = new ItemLocation();
        public ItemLocation LocationFrom { get; set; } = new ItemLocation();
        public bool UseFirstFreeSlot { get; set; }
        public uint StackCount { get; set; } // Value of 0 transfers entire stack

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
            ItemGuidFrom = reader.ReadULong();
            ItemGuidTo = reader.ReadULong();
            Item2IdFrom = reader.ReadUInt();
            LocationTo.Read(reader);
            LocationFrom.Read(reader);
            UseFirstFreeSlot = reader.ReadBit();
            StackCount = reader.ReadUInt();
        }
    }
}
