using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitStealth)]
    public class ServerUnitStealth : IWritable
    {
        public uint UnitId { get; set; }
        public bool Stealthed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Stealthed);
        }
    }
}
