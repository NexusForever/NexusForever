using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStorePurchaseResult)]
    public class ServerStorePurchaseResult : IWritable
    {
        public bool Success { get; set; }
        public StoreError ErrorCode { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Success);
            writer.Write(ErrorCode, 5u);
        }
    }
}
