using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierHoldoutStatus)]
    public class ServerPathSoldierHoldoutStatus : IWritable
    {
        public enum PlayerPathSoldierEventMode
        {
            Inactive = 0,
            Setup = 1,
            InitialDelay = 2,
            Active = 3,
        }

        public uint PathSoldierEventId { get; set; }
        public List<TowerDefenseUnitInfo> UnitInfo { get; set; }
        public uint UnitId { get; set; }
        public bool IsBoss { get; set; }
        public PlayerPathSoldierEventMode Mode { get; set; }
        public int DelayTime { get; set; }
        public int WaveIndex { get; set; }
        public float MaxDefendHealth { get; set; }
        public float MaxAuxiliaryHealth { get; set; }
        public int StartTimeOffset { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(UnitInfo.Count);
            foreach (var unitInfo in UnitInfo)
            {
                unitInfo.Write(writer);
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
