using NexusForever.Game.Static.ICComm;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.ICComm
{
    // Presumably sent if a client is not allowed to join a channel but none seen in sniffs
    [Message(GameMessageOpcode.ServerICCommChannelJoinResult)]
    public class ServerICCommChannelJoinResult : IWritable
    {
        public ICCommChannelType Type { get; set; }
        public ICCommJoinResult Result { get; set; }
        public string ChannelName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 3u);
            writer.Write(Result, 4u);
            writer.WriteStringWide(ChannelName);
        }
    }
}
