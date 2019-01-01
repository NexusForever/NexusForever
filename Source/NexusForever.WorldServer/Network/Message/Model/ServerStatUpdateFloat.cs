using System;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerStatUpdateFloat, MessageDirection.Server)]
    public class ServerStatUpdateFloat : IWritable
    {
        public uint Guid { get; set; }
        public Stat Stat { get; set; }
        public uint Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Stat, 5);
            writer.Write(Value);
        }
    }
}
