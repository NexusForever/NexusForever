using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class TowerDefenseUnitInfo : IWritable
    {
        public enum TowerDefenseUnitType
        {
            Defend = 0,
            Auxiliary = 1,
            Escaping = 2
        }

        public uint UnitId { get; set; }
        public TowerDefenseUnitType Type { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Type);
        }
    }
}
