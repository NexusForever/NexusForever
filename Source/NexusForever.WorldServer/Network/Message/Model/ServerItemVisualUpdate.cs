using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerItemVisualUpdate)]
    public class ServerItemVisualUpdate : IWritable
    {
        public uint Guid { get; set; }
        public List<ItemVisual> ItemVisuals { get; set; } = new List<ItemVisual>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(ItemVisuals.Count);
            ItemVisuals.ForEach(v => v.Write(writer));
        }
    }
}
