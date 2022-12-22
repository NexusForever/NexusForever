using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{

    [Message(GameMessageOpcode.ServerPathActivateResult)]
    public class ServerPathActivateResult : IWritable
    {
        public GenericError Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 8);
        }
    }
}
