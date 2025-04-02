using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierTowerDefenseUnitRemove)]
    public class ServerPathSoldierTowerDefenseUnitRemove : IWritable
    {
        public ushort PathSoldierEventId { get; set; }
        public uint UnitId { get; set; }
        public uint TowerDefenseUnitType { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(UnitId);
            writer.Write(TowerDefenseUnitType);
        }
    }
}
