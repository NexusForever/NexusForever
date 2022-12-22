using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGenericUnlockResult)]
    public class ServerGenericUnlockResult : IWritable
    {
        public GenericUnlockResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 3u);
        }
    }
}
