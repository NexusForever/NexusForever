using NexusForever.Game.Static.Challenges;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Challenges
{
    [Message(GameMessageOpcode.ClientChallengeChoice)]
    public class ClientChallengeChoice : IReadable
    {
        public ushort ChallengeId { get; private set; }
        public ChallengeChoice Choice { get; private set; }
        public uint Unused { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ChallengeId = reader.ReadUShort(14u);
            Choice = reader.ReadEnum<ChallengeChoice>(5u);
            Unused = reader.ReadUInt();
        }
    }
}
