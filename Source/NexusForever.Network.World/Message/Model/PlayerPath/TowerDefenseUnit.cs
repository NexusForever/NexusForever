using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    public class TowerDefenseUnit : IWritable
    {
        public uint UnitId { get; set; }
        public TowerDefenseUnitType Type { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Type, 32u);
        }
    }
}
