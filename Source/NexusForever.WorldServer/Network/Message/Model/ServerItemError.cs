using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    /// <remarks>
    /// Very similar to <see cref="ServerGenericError"/> except only <see cref="GenericError"/> codes
    /// 27, 30, 31, 35, 39, 40, 42, 63, 72, 132 and 156 are handled, anything else defaults to "You can't use that"
    /// </remarks>
    [Message(GameMessageOpcode.ServerItemError)]
    public class ServerItemError : IWritable
    {
        public ulong ItemGuid { get; set; }
        public GenericError ErrorCode { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(ErrorCode);
        }
    }
}
