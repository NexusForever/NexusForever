using NexusForever.Game.Abstract.Account.Reward;
using NexusForever.Game.Static.Rewards;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Rewards;

namespace NexusForever.Game.Account.Reward
{
    public class RewardProperty : IRewardProperty
    {
        public RewardPropertyEntry Entry { get; }

        private readonly Dictionary<uint, float> values = new();

        /// <summary>
        /// Create a new <see cref="IRewardProperty"/> with the supplied <see cref="RewardPropertyEntry"/>.
        /// </summary>
        public RewardProperty(RewardPropertyEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Update the value of the supplied data value.
        /// </summary>
        /// <remarks>
        /// A positive value will increment and a negative value will decrement the value.
        /// </remarks>
        public void UpdateValue(uint data, float value)
        {
            if (!values.ContainsKey(data))
                values.Add(data, value);
            else
                values[data] += value;
        }

        /// <summary>
        /// Set the value of the supplied data value.
        /// </summary>
        public void SetValue(uint data, float value)
        {
            if (!values.ContainsKey(data))
                values.Add(data, value);
            else
                values[data] = value;
        }

        /// <summary>
        /// Get the value of the supplied data value.
        /// </summary>
        public float? GetValue(uint data)
        {
            return values.TryGetValue(data, out float value) ? value : null;
        }

        /// <summary>
        /// Build the network model <see cref="ServerPremiumRewards.RewardProperty"/> for this <see cref="IRewardProperty"/>.
        /// </summary>
        public IEnumerable<ServerPremiumRewards.RewardProperty> Build()
        {
            return values.Select(p => new ServerPremiumRewards.RewardProperty
            {
                RewardPropertyId = (RewardPropertyType)Entry.Id,
                ModifierType     = (RewardModifierValueType)Entry.RewardModifierValueTypeEnum,
                Data  = p.Key,
                Value = p.Value
            });
        }
    }
}
