using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityPropertiesUpdate)]
    public class ServerEntityPropertiesUpdate : IWritable
    {
        public uint UnitId { get; set; }   
        public List<PropertyValue> Properties { get; set; } = new List<PropertyValue>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Properties.Count, 8u);
            Properties.ForEach(u => u.Write(writer));
        }
    }
}
