using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemUse, MessageDirection.Client)]
    public class ClientItemUse : IReadable
    {
        public uint CastingId { get; private set; }
        public InventoryLocation Location { get; private set; } // 9
        public uint BagIndex { get; private set; }
        public uint Unknown3 { get; private set; }
        public ushort Unknown4 { get; private set; } // 9
        public uint Unknown5 { get; private set; }
        public uint Unknown6 { get; private set; }
        public uint Unknown7 { get; private set; }
        public uint Unknown8 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CastingId  = reader.ReadUInt();
            Location = (InventoryLocation)reader.ReadUShort(9u);
            BagIndex = reader.ReadUInt();
            Unknown3 = reader.ReadUInt();
            Unknown4 = reader.ReadUShort(9u);
            Unknown5 = reader.ReadUInt();
            Unknown6 = reader.ReadUInt();
            Unknown7 = reader.ReadUInt();
            Unknown8 = reader.ReadUInt();
        }
    }
}
