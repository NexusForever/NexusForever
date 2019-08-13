using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerInnateSet)]
    public class ServerPlayerInnate : IWritable
    {
        public byte InnateIndex { get; set; }
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(InnateIndex, 2u);
        }
    }
}
