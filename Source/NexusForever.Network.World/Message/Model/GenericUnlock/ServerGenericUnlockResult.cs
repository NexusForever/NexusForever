using NexusForever.Game.Static.GenericUnlock;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.GenericUnlock
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
