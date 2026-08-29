using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathSoldierHoldoutStatus)]
    public class ServerPathSoldierHoldoutStatus : IWritable
    {
        public uint PathSoldierEventId { get; set; }
        public List<TowerDefenseUnit> Units { get; set; } = [];
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
            writer.Write(Units.Count);
            Units.ForEach(unit => unit.Write(writer));
            writer.Write(UnitId);
            writer.Write(IsBoss);
            writer.Write(Mode, 32u);
            writer.Write(DelayTime);
            writer.Write(WaveIndex);
            writer.Write(MaxDefendHealth);
            writer.Write(MaxAuxiliaryHealth);
            writer.Write(StartTimeOffset);
        }
    }
}
