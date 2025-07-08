using NexusForever.Game.Static.Setting;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerShowInstanceGameModeDialog)]
    public class ServerShowInstanceGameModeDialog : IWritable
    {
        public uint InstancePortalUnitId { get; set; }
        public WorldDifficulty Difficulty { get; set; }
        public InstanceParameters Parameters { get; set; }
        public WorldDifficulty ExistingDifficulty { get; set; }
        public byte MaxPrimeLevelWorld { get; set; }
        public byte MaxPrimeLevelGroup { get; set; }
        public byte ScalingLevel { get; set; }
        public bool ExistingScaling { get; set; }
        public byte ExistingPrimeLevel { get; set; }
        public bool IsRaid { get; set; }
        public bool PrimeAllowed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(InstancePortalUnitId);
            writer.Write(Difficulty, 2u);
            writer.Write(Parameters,11u);
            writer.Write(ExistingDifficulty, 2u);
            writer.Write(MaxPrimeLevelWorld);
            writer.Write(MaxPrimeLevelGroup);
            writer.Write(ScalingLevel);
            writer.Write(ExistingScaling);
            writer.Write(ExistingPrimeLevel);
            writer.Write(IsRaid);
            writer.Write(PrimeAllowed);
        }
    }
}
