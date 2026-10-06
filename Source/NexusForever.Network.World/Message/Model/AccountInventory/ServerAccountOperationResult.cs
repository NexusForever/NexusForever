using NexusForever.Game.Static.AccountInventory;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountOperationResult)]
    public class ServerAccountOperationResult : IWritable
    {
        public AccountOperation Operation { get; set; }
        public AccountOperationResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Operation);
            writer.Write(Result);
        }
    }
}
