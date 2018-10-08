using System;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server019D, MessageDirection.Server)]
    public class Server019D : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
