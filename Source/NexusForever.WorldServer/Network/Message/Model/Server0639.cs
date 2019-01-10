using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0639, MessageDirection.Server)]
    public class Server0639 : IWritable
    {
        public uint Unknown0 { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 0u);
        }
    }
}
