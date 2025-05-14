using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingAddAdditive)]
    public class ClientCraftingAddAdditive : IReadable
    {
        public uint CraftingStationUnitId { get; private set; }
        public uint AdditiveItem2Id { get; private set; }
        public uint CatalystItem2Id { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CraftingStationUnitId = reader.ReadUInt();
            AdditiveItem2Id = reader.ReadUInt(18);
            CatalystItem2Id = reader.ReadUInt(18);
        }
    }
}
