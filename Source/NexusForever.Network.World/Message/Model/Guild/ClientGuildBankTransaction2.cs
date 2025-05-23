using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildBankTransaction2)]
    public class ClientGuildBankTransaction2 : IReadable
    {
        public Identity GuildIdentity { get; set; } = new Identity();
        public ulong SourceItemGuid { get; set; }
        public ulong DestinationItemGuid { get; set; } // Sent if bag slot already contains an item, otherwise 0
        public uint SourceItem2Id { get; set; }
        public ItemLocation Destination { get; set; } = new ItemLocation();
        public ItemLocation Source { get; set; } = new ItemLocation();
        public bool UseFirstFreeSlot { get; set; }
        public uint Count { get; set; } // Value of 0 transfers entire stack

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
            SourceItemGuid = reader.ReadULong();
            DestinationItemGuid = reader.ReadULong();
            SourceItem2Id = reader.ReadUInt();
            Destination.Read(reader);
            Source.Read(reader);
            UseFirstFreeSlot = reader.ReadBit();
            Count = reader.ReadUInt();
        }
    }
}
