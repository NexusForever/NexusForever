using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChallengeShared)]
    public class ServerChallengeShared : IWritable
    {
        public ushort ChallengeId { get; set; }
        public uint SharerUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChallengeId, 14u);
            writer.Write(SharerUnitId);
        }
    }
}
