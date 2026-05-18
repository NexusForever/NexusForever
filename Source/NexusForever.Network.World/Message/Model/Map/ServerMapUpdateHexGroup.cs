using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Map
{
    [Message(GameMessageOpcode.ServerMapUpdateHexGroup)]
    public class ServerMapUpdateHexGroup : IWritable
    {
        public uint MapZoneHexGroupId { get; set; }
        public uint TooltipLocalizedTextId { get; set; }
        public uint Color { get; set; }
        public bool IsVisible { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MapZoneHexGroupId, 14u);
            writer.Write(TooltipLocalizedTextId, 21u);
            writer.Write(Color);
            writer.Write(IsVisible);
        }
    }
}
