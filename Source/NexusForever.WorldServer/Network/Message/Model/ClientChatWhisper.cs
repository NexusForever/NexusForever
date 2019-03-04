using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientChatWhisper)]
    public class ClientChatWhisper : IReadable
    {
        public string PlayerName { get; private set; }
        public string Unknown0 { get; private set; }

        public string Message { get; private set; }
        public List<ChatFormat> Formats { get; } = new List<ChatFormat>();

        public bool Unknown1 { get; set; }

        public void Read(GamePacketReader reader)
        {
            PlayerName = reader.ReadWideString();
            Unknown0 = reader.ReadWideString();

            Message  = reader.ReadWideString();
            byte formatCount = reader.ReadByte(5u);
            for (int i = 0; i < formatCount; i++)
            {
                var format = new ChatFormat();
                format.Read(reader);
                Formats.Add(format);
            }

            Unknown1 = reader.ReadBit();
        }
    }
}
