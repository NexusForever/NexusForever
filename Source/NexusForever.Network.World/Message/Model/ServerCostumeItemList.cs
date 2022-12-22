using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeItemList)]
    public class ServerCostumeItemList : IWritable
    {
        public List<uint> Items { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Items.Count);
            Items.ForEach(i => writer.Write(i));
        }
    }
}
