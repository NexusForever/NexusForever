using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierAddTowerDefense)]
    public class ServerPathSoldierAddTowerDefense : IWritable
    {
        public ushort PathSoldierEventId { get; set; }
        public ushort PathSoldierTowerDefenseId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(PathSoldierTowerDefenseId, 14);
        }
    }
}
