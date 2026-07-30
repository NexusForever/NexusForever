using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemMicrochips)]
    public class ServerItemMicrochips : IWritable
    {
        public ulong ItemGuid { get; set; }
        public uint MakerCharacterId { get; set; }
        public CraftStats CircuitData { get; set; } = new();
        public uint PowerCoreItem2Id { get; set; }
        public uint[] MicrochipItem2Ids { get; set; } = new uint[5];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(MakerCharacterId);
            CircuitData.Write(writer);
            writer.Write(PowerCoreItem2Id, 18u);
            writer.Write(MicrochipItem2Ids.Length, 4u);
            foreach (uint glyphItem2Id in MicrochipItem2Ids)
            {
                writer.Write(glyphItem2Id);
            }
        }
    }
}
