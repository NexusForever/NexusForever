using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Share timeout is set at 30 seconds per GameFormulaId 0x416
    [Message(GameMessageOpcode.ServerChallengeShareTimeout)]
    public class ServerChallengeShareTimeout : IWritable
    {
        public ushort ChallengeId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChallengeId, 14u);
        }
    }
}
