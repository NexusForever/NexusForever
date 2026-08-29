using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // Send to all observers in the nearby area. Client handles the correctness of the UI messages.
    [Message(GameMessageOpcode.ServerDuelChallenge)]
    public class ServerDuelChallenge : IWritable
    {
        public uint ChallengerUnitId { get; set; }
        public uint OpponentUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChallengerUnitId);
            writer.Write(OpponentUnitId);
        }
    }
}
