using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChatJoin)]
    public class ServerChatJoin : IWritable
    {
        public ChatChannelType Channel { get; set; }
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
