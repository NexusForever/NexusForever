using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathSoldierImprovement_Build)]
    public class ClientPathSoldierImprovement_Build : IReadable
    {
        public ushort PathSoldierTowerDefenseId { get; set; }

        public void Read(GamePacketReader reader)
        {
            PathSoldierTowerDefenseId = reader.ReadUShort(14);
        }
    }
}
