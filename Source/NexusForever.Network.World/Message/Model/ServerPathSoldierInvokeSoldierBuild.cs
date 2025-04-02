using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierInvokeSoldierBuild)]
    public class ServerPathSoldierInvokeSoldierBuild : IWritable
    {
        public uint UnitId { get; set; }
        public ushort PathSoldierEventId { get; set; }
        public uint[] TowerDefenseIds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(PathSoldierEventId, 14);
            writer.Write(TowerDefenseIds.Length);
            foreach(uint id in TowerDefenseIds)
            {
                writer.Write(id);
            }
        }
    }
}
