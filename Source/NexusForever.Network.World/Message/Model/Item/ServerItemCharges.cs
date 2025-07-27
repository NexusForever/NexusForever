using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemCharges)]
    public class ServerItemCharges : IWritable
    {
        public ulong ItemGuid { get; set; }
        public uint Charges { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(Charges);
        }
    }
}
