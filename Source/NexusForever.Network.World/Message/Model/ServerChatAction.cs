using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
