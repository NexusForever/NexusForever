using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerItemVisualUpdate)]
    public class ServerItemVisualUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public List<ItemVisual> ItemVisuals { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(ItemVisuals.Count);
            ItemVisuals.ForEach(v => v.Write(writer));
        }
    }
}
