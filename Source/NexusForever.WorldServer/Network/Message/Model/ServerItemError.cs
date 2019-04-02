using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerItemError)]
    public class ServerItemError : IWritable
    {
        public ulong ItemGuid { get; set; }
        public ItemError ErrorCode { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(ErrorCode);
        }
    }
}
