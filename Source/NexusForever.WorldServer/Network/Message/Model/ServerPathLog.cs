﻿using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPathLog, MessageDirection.Server)]
    public class ServerPathLog : IWritable
    {

        public Path ActivePath { get; set; }
        public uint[] PathProgress { get; set; } = new uint[4];
        public PathUnlockedMask PathUnlockedMask { get; set; }
        public int ActivateTimer { get; set; } // > 0 = On. < 0 = Off.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ActivePath, 3);

            for (uint i = 0u; i < PathProgress.Length; i++)
                writer.Write(PathProgress[i]);

            writer.Write(PathUnlockedMask, 4);
            writer.Write(ActivateTimer);
        }
    }
}
