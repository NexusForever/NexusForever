using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierHoldoutStatus)]
    public class ServerPathSoldierHoldoutStatus : IWritable
    {
        public class TowerDefenseUnitInfo
        {
            public enum TowerDefenseUnitType
            {
                Defend = 0,
                Auxiliary = 1,
                Escaping = 2
            }

            public uint UnitId { get; set; }
            public TowerDefenseUnitType Type { get; set; }
        }

        public enum PlayerPathSoldierEventMode
        {
            Inactive = 0,
            Setup = 1,
            InitialDelay = 2,
            Active = 3,
        }

        public uint PathSoldierEventId;
        public List<TowerDefenseUnitInfo> UnitInfo;
        public uint UnitId;
        public bool IsBoss;
        PlayerPathSoldierEventMode Mode;
        public int DelayTime;
        public int WaveIndex;
        public float MaxDefendHealth;
        public float MaxAuxiliaryHealth;
        public int StartTimeOffset;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(UnitInfo.Count);
            foreach (var unitInfo in UnitInfo)
            {
                writer.Write(unitInfo.UnitId);
                writer.Write(unitInfo.Type);
            }
            writer.Write(UnitId);
            writer.Write(IsBoss);
            writer.Write(Mode);
            writer.Write(DelayTime);
            writer.Write(WaveIndex);
            writer.Write(MaxDefendHealth);
            writer.Write(MaxAuxiliaryHealth);
            writer.Write(StartTimeOffset);
        }
    }
}
