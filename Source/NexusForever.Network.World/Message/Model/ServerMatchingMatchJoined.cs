using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchJoined)]
    public class ServerMatchingMatchJoined : IWritable
    {
        public uint MatchingGameMap { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MatchingGameMap, 0xEu);
        }
    }
}
