using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerItemVisualUpdate)]
    public class ServerItemVisualUpdate : IWritable
    {
        public uint Guid { get; set; }
        public List<ItemVisual> ItemVisuals { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(ItemVisuals.Count);
            ItemVisuals.ForEach(v => v.Write(writer));
        }
    }
}
