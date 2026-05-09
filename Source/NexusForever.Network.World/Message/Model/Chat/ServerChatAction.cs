using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChatAction)]
    public class ServerChatAction : IWritable
    {
        public Channel Channel { get; set; }
        public string NameActor { get; set; }
        public string NameActedOn { get; set; }
        public ChatChannelAction Action { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);
            writer.WriteStringWide(NameActor);
            writer.WriteStringWide(NameActedOn);
            writer.Write(Action, 4u);
        }
    }
}
