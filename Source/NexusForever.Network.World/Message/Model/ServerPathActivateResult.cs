using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{

    [Message(GameMessageOpcode.ServerPlayerPathChangeResult)]
    public class ServerPlayerPathChangeResult : IWritable
    {
        public GenericError Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 8);
        }
    }
}
