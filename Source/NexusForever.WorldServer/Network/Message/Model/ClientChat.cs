using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientChat)]
    public class ClientChat : IReadable
    {
        public ChatChannel Channel { get; private set; }
        public ulong Unknown0 { get; set; }
        public string Message { get; private set; }
        public List<ChatFormat> Formats { get; } = new List<ChatFormat>();

        public void Read(GamePacketReader reader)
        {
            Channel  = reader.ReadEnum<ChatChannel>(14u);
            Unknown0 = reader.ReadULong();
            Message  = reader.ReadWideString();

            byte formatCount = reader.ReadByte(5u);
            for (int i = 0; i < formatCount; i++)
            {
                var format = new ChatFormat();
                format.Read(reader);
                Formats.Add(format);
            }
        }
    }
}
