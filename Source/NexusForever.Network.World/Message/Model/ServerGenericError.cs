using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGenericError)]
    public class ServerGenericError : IWritable
    {
        public GenericError Error { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Error, 8u);
        }
    }
}
