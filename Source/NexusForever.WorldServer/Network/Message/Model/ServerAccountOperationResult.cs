using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Account.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountOperationResult)]
    public class ServerAccountOperationResult : IWritable
    {
        public AccountOperation Operation { get; set; }
        public AccountOperationResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Operation, 32u);
            writer.Write(Result, 32u);
        }
    }
}
