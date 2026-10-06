using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Spell
{
    public class SpellInit : IWritable
    {
        public uint CasterUnitId { get; set; }
        public uint TargetUnitId { get; set; }
        public uint ServerSpellCastUniqueId { get; set; }
        public uint Spell4Id { get; set; }
        public bool BIgnoreCooldown { get; set; }
        public List<TargetInfo> TargetInfoData { get; set; } = new();
        public List<InitialPosition> InitialPositionData { get; set; } = new();
        public List<TelegraphPosition> TelegraphPositionData { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterUnitId);
            writer.Write(TargetUnitId);
            writer.Write(ServerSpellCastUniqueId);
            writer.Write(Spell4Id, 18u);
            writer.Write(BIgnoreCooldown);

            writer.Write(TargetInfoData.Count, 32u);
            TargetInfoData.ForEach(u => u.Write(writer));

            writer.Write(InitialPositionData.Count, 8u);
            InitialPositionData.ForEach(u => u.Write(writer));

            writer.Write(TelegraphPositionData.Count, 8u);
            TelegraphPositionData.ForEach(u => u.Write(writer));
        }
    }
}
