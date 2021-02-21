using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
