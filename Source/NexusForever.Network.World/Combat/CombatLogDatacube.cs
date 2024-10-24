using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogDatacube : ICombatLog
    {
        public CombatLogType Type => CombatLogType.Datacube;

        public uint UnitId { get; set; }
        public byte DatacubeType { get; set; } // 3u
        public bool HasPieces { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(DatacubeType, 3u);
            writer.Write(HasPieces);
        }
    }
}
