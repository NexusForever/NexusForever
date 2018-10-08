using System;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server01A0, MessageDirection.Server)]
    public class Server01A0 : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
