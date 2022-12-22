using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
