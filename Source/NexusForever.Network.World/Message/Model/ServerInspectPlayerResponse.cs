using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerInspectPlayerResponse)]
    public class ServerInspectPlayerResponse : IWritable
    {
        public uint Guid { get; set; }
        public List<Item> Items { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);

            writer.Write(Items.Count, 5u);
            Items.ForEach(item => item.Write(writer));
        }
    }
}
