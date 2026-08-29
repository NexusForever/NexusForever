using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ClientGuildBankTransaction2)]
    public class ClientGuildBankTransaction2 : IReadable
    {
        public Identity GuildIdentity { get; private set; } = new Identity();
        public ulong SourceItemGuid { get; private set; }
        public ulong DestinationItemGuid { get; private set; } // Sent if bag slot already contains an item, otherwise 0
        public uint SourceItem2Id { get; private set; }
        public ItemLocation Destination { get; private set; } = new ItemLocation();
        public ItemLocation Source { get; private set; } = new ItemLocation();
        public bool UseFirstFreeSlot { get; private set; }
        public uint Count { get; private set; } // Value of 0 transfers entire stack

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
