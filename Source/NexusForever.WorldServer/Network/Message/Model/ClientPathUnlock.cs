﻿using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPathUnlock, MessageDirection.Client)]
    public class ClientPathUnlock : IReadable
    {
        public Path Path { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Path = reader.ReadEnum<Path>(3);
        }
    }
}
