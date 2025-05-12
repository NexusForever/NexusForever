using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierHoldOutNextWave)]
    public class ServerPathSoldierHoldOutNextWave : IWritable
    {
        public ushort PathSoldierEventId { get; set; }
        public uint WaveIndex { get; set; }
        public bool IsBoss { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(WaveIndex);
            writer.Write(IsBoss);
;        }
    }
}
