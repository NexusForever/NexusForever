using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityPropertyBatchUpdate, MessageDirection.Server)]
    public class ServerEntityPropertyBatchUpdate : IWritable
    {
        public uint Guid { get; set; }
        public List<PropertyValue> PropertyValues { get; } = new List<PropertyValue>();
        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(PropertyValues.Count, 8);
            foreach(PropertyValue value in PropertyValues)
                value.Write(writer);
        }
    }
}
