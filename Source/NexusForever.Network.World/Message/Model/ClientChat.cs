﻿using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientChat)]
    public class ClientChat : IReadable
    {
        public Channel Channel { get; private set; }
        public string Message { get; private set; }
        public List<ChatFormat> Formats { get; } = new();
        public ushort Unknown0C { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel = new Channel();
            Channel.Read(reader);
            Message = reader.ReadWideString();

            byte formatCount = reader.ReadByte(5u);
            for (int i = 0; i < formatCount; i++)
            {
                var format = new ChatFormat();
                format.Read(reader);
                Formats.Add(format);
            }

            Unknown0C = reader.ReadUShort();
        }
    }
}
