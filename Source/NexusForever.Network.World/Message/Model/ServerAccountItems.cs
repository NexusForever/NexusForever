using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountItems)]
    public class ServerAccountItems : IWritable
    {
        public List<AccountInventoryItem> AccountItems { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountItems.Count);
            AccountItems.ForEach(w => w.Write(writer));
        }
    }
}
