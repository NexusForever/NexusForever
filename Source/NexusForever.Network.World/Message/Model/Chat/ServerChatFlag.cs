using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChatFlag)]
    public class ServerChatFlag : IWritable
    {
        public Channel ChatChannelId { get; set; }
        public ulong ChannelGuid { get; set; }
        public ChatChannelMemberFlags Flags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            ChatChannelId.Write(writer);
            writer.Write(ChannelGuid);
            writer.Write(Flags, 32u);
        }
    }
}
