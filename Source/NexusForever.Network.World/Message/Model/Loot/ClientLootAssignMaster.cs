using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Loot
{
    [Message(GameMessageOpcode.ClientLootAssignMaster)]
    public class ClientLootAssignMaster : IReadable
    {
        public uint OwnerUnitId { get; private set; }
        public uint LootUnitId { get; private set; }
        public Identity Assignee { get; private set; } = new Identity();

        public void Read(GamePacketReader reader)
        {
            OwnerUnitId = reader.ReadUInt();
            LootUnitId = reader.ReadUInt();
            Assignee.Read(reader);
        }
    }
}
