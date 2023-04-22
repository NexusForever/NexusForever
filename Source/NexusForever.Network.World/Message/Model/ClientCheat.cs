﻿using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCheat)]
    public class ClientCheat : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            Message = reader.ReadWideString();
        }

        public string Message { get; private set; }
    }
}