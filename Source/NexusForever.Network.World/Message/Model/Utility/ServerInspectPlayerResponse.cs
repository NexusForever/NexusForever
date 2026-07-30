using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Utility
{
    [Message(GameMessageOpcode.ServerInspectPlayerResponse)]
    public class ServerInspectPlayerResponse : IWritable
    {
        public uint UnitId { get; set; }
        public List<Item.Item> Items { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);

            writer.Write(Items.Count, 5u);
            Items.ForEach(item => item.Write(writer));
        }
    }
}
