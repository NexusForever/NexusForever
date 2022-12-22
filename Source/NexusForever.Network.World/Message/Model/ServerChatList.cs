using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChatList)]
    public class ServerChatList : IWritable
    {
        public ChatChannelType Type { get; set; }
        public ulong ChannelId { get; set; }
        public List<string> Names { get; set; }
        public List<ChatChannelMemberFlags> Flags { get; set; }
        public bool More { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 14u);
            writer.Write(ChannelId);

            writer.Write(Names.Count);
            foreach (string name in Names)
                writer.WriteStringWide(name);
            foreach (ChatChannelMemberFlags flag in Flags)
                writer.Write(flag, 32u);
        }
    }
}
