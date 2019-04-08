using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingVendorList)]
    public class ServerHousingVendorList : IWritable
    {
        public class PlugItem : IWritable
        {
            public ulong SourceId { get; set; }
            public uint PlugItemId { get; set; }
            public uint Cost { get; set; }
            public uint PlugItemFlags { get; set; }
            
            public void Write(GamePacketWriter writer)
            {
                writer.Write(SourceId);
                writer.Write(PlugItemId);
                writer.Write(Cost);
                writer.Write(PlugItemFlags);
            }
        }

        public List<PlugItem> PlugItems { get; set; } = new List<PlugItem>();
        public byte ListType { get; set; }
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(PlugItems.Count);
            PlugItems.ForEach(p => p.Write(writer));
            writer.Write(ListType, 2);
        }
    }
}
