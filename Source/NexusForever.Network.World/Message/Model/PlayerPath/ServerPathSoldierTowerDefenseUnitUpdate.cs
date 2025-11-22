using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathSoldierTowerDefenseUnitUpdate)]
    public class ServerPathSoldierTowerDefenseUnitUpdate : IWritable
    {
        public ushort PathSoldierEventId { get; set; }
        public uint UnitId { get; set; }
        public TowerDefenseUnitType Type { get; set; }
        public float MaxHealth { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(UnitId);
            writer.Write(Type, 32u);
            writer.Write(MaxHealth);
        }
    }
}
