using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildBankTransaction)]
    public class ClientGuildBankTransaction : IReadable
    {
        public Identity GuildIdentity { get; set; } = new Identity();
        public ulong CharacterItemGuidFrom { get; set; }
        public ulong CharacterItemGuidTo { get; set; } // Sent if bag slot already contains an item, otherwise 0
        public ItemLocation LocationFrom { get; set; } = new ItemLocation();
        public ItemLocation LocationTo { get; set; } = new ItemLocation();
        public uint StackCount { get; set; } // Value of 0 transfers entire stack

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
            CharacterItemGuidFrom = reader.ReadULong();
            CharacterItemGuidTo = reader.ReadULong();
            LocationFrom.Read(reader);
            LocationTo.Read(reader);
            StackCount = reader.ReadUInt();
        }
    }
}
