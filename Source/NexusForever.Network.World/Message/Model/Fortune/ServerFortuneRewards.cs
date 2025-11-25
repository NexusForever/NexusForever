using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortune
{
    [Message(GameMessageOpcode.ServerFortuneRewards)]
    public class ServerFortuneRewards : IWritable
    {
        public class MoneyReward : IWritable
        {
            public byte SecondaryCurrencyAmount { get; set; }
            public uint MoneyAmount { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(SecondaryCurrencyAmount, 5u);
                writer.Write(MoneyAmount);
            }
        }

        public List<uint> Item2IdRewards { get; set; } = [];
        public List<MoneyReward> MoneyRewards { get; set; } = [];       // not seen used in sniffs
        public List<float> RewardItemProbabilities { get; set; } = [];  // not seen used in sniffs
        public List<float> RewardMoneyProbabilities { get; set; } = []; // not seen used in sniffs

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Item2IdRewards.Count);
            Item2IdRewards.ForEach(reward => writer.Write(reward));
            writer.Write(MoneyRewards.Count);
            MoneyRewards.ForEach(reward => reward.Write(writer));
            writer.Write(RewardItemProbabilities.Count);
            RewardItemProbabilities.ForEach(reward => writer.Write(reward));
            writer.Write(RewardMoneyProbabilities.Count);
            RewardMoneyProbabilities.ForEach(reward => writer.Write(reward));
        }
    }
}
