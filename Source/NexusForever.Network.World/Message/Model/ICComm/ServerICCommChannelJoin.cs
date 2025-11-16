using NexusForever.Game.Static.ICComm;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.ICComm
{
    [Message(GameMessageOpcode.ServerICCommChannelJoin)]
    public class ServerICCommChannelJoin : IWritable
    {
        public ICCommChannelType Type { get; set; }
        public ulong IccomId { get; set; }
        public string ChannelName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 3u);
            writer.Write(IccomId);
            writer.WriteStringWide(ChannelName);
        }
    }
}
