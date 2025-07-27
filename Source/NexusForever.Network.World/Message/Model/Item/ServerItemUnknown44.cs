using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemUnknown44)]
    public class ServerItemUnknown44 : IWritable
    {
        public ulong ItemGuid { get; set; }
        public uint Unknown44 { get; set; } // is the Unknown44 field in the Item message (0x111), possibly costume related

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(Unknown44);
        }
    }
}
