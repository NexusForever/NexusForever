using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemDyteData)]
    public class ServerItemDyteData : IWritable
    {
        public ulong ItemGuid { get; set; }
        public uint DyeData { get; set; } // see GenerateDyeMask in CostumeItem

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(DyeData);
        }
    }
}
