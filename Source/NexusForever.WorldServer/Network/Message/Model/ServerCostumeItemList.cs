using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeItemList)]
    public class ServerCostumeItemList : IWritable
    {
        public List<uint> Items { get; set; } = new List<uint>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Items.Count);
            Items.ForEach(i => writer.Write(i));
        }
    }
}
