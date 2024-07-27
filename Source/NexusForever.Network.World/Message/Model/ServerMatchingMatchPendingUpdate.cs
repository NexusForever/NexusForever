using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchPendingUpdate)]
    public class ServerMatchingMatchPendingUpdate : IWritable
    {
        public bool Ally { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Ally);
        }
    }
}
