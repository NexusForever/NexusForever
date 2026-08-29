using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathScientistUnitScanParameters)]
    public class ServerPathScientistUnitScanParameters : IWritable
    {
        public uint UnitId { get; set; }
        public ScanReward ScanRewardFlags { get; set; } 
        public bool IsScannable { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(ScanRewardFlags, 32u);
            writer.Write(IsScannable);
        }
    }
}
