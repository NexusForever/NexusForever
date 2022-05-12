using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStoreError)]
    public class ServerStoreError : IWritable
    {
        public StoreError ErrorCode { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ErrorCode, 5u);
        }
    }
}
