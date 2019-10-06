using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerInspectPlayerResponse)]
    public class ServerInspectPlayerResponse : IWritable
    {
        public uint Guid { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);

            writer.Write(Items.Count, 5u);
            Items.ForEach(item => item.Write(writer));
        }
    }
}
