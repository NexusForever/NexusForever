using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathSoldierImprovementBuild)]
    public class ClientPathSoldierImprovementBuild : IReadable
    {
        public ushort PathSoldierTowerDefenseId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PathSoldierTowerDefenseId = reader.ReadUShort(14);
        }
    }
}
