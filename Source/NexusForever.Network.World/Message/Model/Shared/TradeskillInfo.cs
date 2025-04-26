using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class TradeskillInfo : IWritable
    {
        public TradeskillType TradeskillId { get; set; }
        public uint TradeskillXp { get; set; }
        public uint IsActive { get; set; }
        public uint TradeskillBonusId { get; set; }
        public uint TalentPoints { get; set; }
        public uint[] TradeskillTalentTierIds { get; set; } = new uint[10];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillId,32u);
            writer.Write(TradeskillXp);
            writer.Write(IsActive);
            writer.Write(TradeskillBonusId);
            writer.Write(TalentPoints);
            foreach (var tierId in TradeskillTalentTierIds)
            {
                writer.Write(tierId);
            }
        }
    }
}
