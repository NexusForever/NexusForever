using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    // Loads any property up to an Property enum value of 196. For values 197, 198, 199 use message 0x93D.
    [Message(GameMessageOpcode.ServerEntityPropertiesUpdate)]
    public class ServerEntityPropertiesUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public List<PropertyValue> Properties { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Properties.Count, 8u);
            Properties.ForEach(u => u.Write(writer));
        }
    }
}
