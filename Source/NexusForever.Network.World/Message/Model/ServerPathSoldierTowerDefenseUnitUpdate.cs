using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierTowerDefenseUnitUpdate)]
    public class ServerPathSoldierTowerDefenseUnitUpdate : IWritable
    {
        public enum TowerDefenseUnitType
        {
            Defend = 0,
            Auxiliary = 1,
            Escaping = 2
        }

        public ushort PathSoldierEventId { get; set; }
        public uint UnitId { get; set; }
        public TowerDefenseUnitType Type { get; set; }
        public float MaxHealth { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(UnitId);
            writer.Write(Type);
            writer.Write(MaxHealth);
        }
    }
}
