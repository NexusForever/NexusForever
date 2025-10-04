using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Loot
{
    [Message(GameMessageOpcode.ClientLootRollAction)]
    public class ClientLootRollAction : IReadable
    {
        public uint OwnerUnitId { get; private set; }
        public uint LootUnitId { get; private set; }
        public LootRollAction Action { get; private set; }

        public void Read(GamePacketReader reader)
        {
            OwnerUnitId = reader.ReadUInt();
            LootUnitId = reader.ReadUInt();
            Action = reader.ReadEnum<LootRollAction>(2u);
        }
    }
}
