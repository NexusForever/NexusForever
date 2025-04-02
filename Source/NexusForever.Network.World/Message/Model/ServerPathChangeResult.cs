using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{

    [Message(GameMessageOpcode.ServerPathChangeResult)]
    public class ServerPathChangeResult : IWritable
    {
        public GenericError Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 8);
        }
    }
}
