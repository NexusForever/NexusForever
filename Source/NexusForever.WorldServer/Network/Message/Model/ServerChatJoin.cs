using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChatJoin)]
    class ServerChatJoin : IWritable
    {
        public ChatChannel Channel { get; set; }
        public ulong ChannelId { get; set; }
        public string CustomChannelName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel, 14u);
            writer.Write(ChannelId);
            writer.WriteStringWide(CustomChannelName);

            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
        }
    }
}
