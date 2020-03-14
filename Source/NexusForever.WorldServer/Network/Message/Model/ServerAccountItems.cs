using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountItems)]
    public class ServerAccountItems : IWritable
    {
        public List<AccountInventoryItem> AccountItems { get; set; } = new List<AccountInventoryItem>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountItems.Count);
            AccountItems.ForEach(w => w.Write(writer));
        }
    }
}
