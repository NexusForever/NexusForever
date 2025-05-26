using NexusForever.Game.Static.Challenges;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChallengeResult)]
    public class ServerChallengeResult : IWritable
    {
        public ushort ChallengeId { get; set; }
        public ChallengeResult Result { get; set; }
        public int Data { get; set; } // Sometimes localizedStringId, sometimes tier value

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChallengeId, 14u);
            writer.Write(Result);
            writer.Write(Data);
        }
    }
}
