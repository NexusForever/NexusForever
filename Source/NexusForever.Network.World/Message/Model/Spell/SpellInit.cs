using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Spell
{
    public class SpellInit : IWritable
    {
        public uint CasterId { get; set; }
        public uint OriginalTargetId { get; set; }
        public uint ServerUniqueId { get; set; }
        public uint SpellId { get; set; }
        public bool Unknown10 { get; set; }
        public List<TargetInfo> TargetInfoData { get; set; } = [];
        public List<InitialPosition> InitialPositionData { get; set; } = [];
        public List<TelegraphPosition> TelegraphPositionData { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterId);
            writer.Write(OriginalTargetId);
            writer.Write(ServerUniqueId);
            writer.Write(SpellId, 18u);
            writer.Write(Unknown10);

            writer.Write(TargetInfoData.Count, 32u);
            TargetInfoData.ForEach(u => u.Write(writer));

            writer.Write(InitialPositionData.Count, 8u);
            InitialPositionData.ForEach(u => u.Write(writer));

            writer.Write(TelegraphPositionData.Count, 8u);
            TelegraphPositionData.ForEach(u => u.Write(writer));
        }
    }
}
