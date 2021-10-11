using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChatResult)]
    public class ServerChatResult : IWritable
    {
        public Channel Channel { get; set; }
        public ChatResult ChatResult { get; set; }
        public ushort Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);
            writer.Write(ChatResult, 5u);
            writer.Write(Unknown0);
        }
    }
}
