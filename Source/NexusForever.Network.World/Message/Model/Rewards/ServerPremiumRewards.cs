using NexusForever.Game.Static.Rewards;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Rewards
{
    [Message(GameMessageOpcode.ServerPremiumRewards)]
    public class ServerPremiumRewards : IWritable
    {
        public class RewardProperty : IWritable
        {
            public class RewardOwnerMultiplier : IWritable
            {
                public RewardModifierOwner OwnerType { get; set; }
                public uint OwnerId { get; set; }
                public RewardModifierValueType ModifierType { get; set; }
                public float Value { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(OwnerType, 4u);
                    writer.Write(OwnerId);
                    writer.Write(ModifierType, 2u);

                    switch (ModifierType)
                    {
                        case RewardModifierValueType.AdditiveScalar:
                        case RewardModifierValueType.MultiplicativeScalar:
                            writer.Write(Value);
                            break;
                        case RewardModifierValueType.Discrete:
                            writer.Write((uint)Value);
                            break;
                    }
                }
            }

            public RewardPropertyType RewardPropertyId { get; set; }
            public uint Data { get; set; }
            public RewardModifierValueType ModifierType { get; set; }
            public float Value { get; set; }
            public List<RewardOwnerMultiplier> OwnerMultipliers { get; set; } = [];

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RewardPropertyId, 6u);
                writer.Write(Data);
                writer.Write(ModifierType, 2u);

                switch (ModifierType)
                {
                    case RewardModifierValueType.AdditiveScalar:
                    case RewardModifierValueType.MultiplicativeScalar:
                        writer.Write(Value);
                        break;
                    case RewardModifierValueType.Discrete:
                        writer.Write((uint)Value);
                        break;
                }

                writer.Write(OwnerMultipliers.Count, 8u);
                OwnerMultipliers.ForEach(u => u.Write(writer));
            }
        }

        public List<RewardProperty> Properties { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write((byte)Properties.Count);
            Properties.ForEach(u => u.Write(writer));
        }
    }
}
