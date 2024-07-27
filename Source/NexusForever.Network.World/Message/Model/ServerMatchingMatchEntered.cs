using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchEntered)]
    public class ServerMatchingMatchEntered : IWritable
    {
        public uint MatchingGameMap { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MatchingGameMap);
        }
    }
}
