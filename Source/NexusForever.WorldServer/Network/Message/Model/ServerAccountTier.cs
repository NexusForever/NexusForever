using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountTier)]
    public class ServerAccountTier : IWritable
    {
        public byte Tier { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Tier, 5u);
        }
    }
}
