using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChatResult)]
    class ServerChatResult : IWritable
    {
        public ChatChannel Channel { get; set; }
        public ulong ChatId { get; set; }
        public ChatResult ChatResult { get; set; }
        public ushort Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel, 14u);
            writer.Write(ChatId);
            writer.Write(ChatResult, 5u);
            writer.Write(Unknown0);
        }
    }
}
