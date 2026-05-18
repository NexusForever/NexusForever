using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchJoined)]
    public class ServerMatchingMatchJoined : IWritable
    {
        public uint MatchingGameMapId { get; set; } // Id of map the player has just joined

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MatchingGameMapId, 0xEu);
        }
    }
}
