using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class RewardProperty : IBuildable<IEnumerable<ServerRewardPropertySet.RewardProperty>>
    {
        public RewardPropertyEntry Entry { get; }

        private readonly Dictionary<uint, float> values = new();

        /// <summary>
        /// Create a new <see cref="RewardProperty"/> with the supplied <see cref="RewardPropertyEntry"/>.
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
            return values.TryGetValue(data, out float value) ? value : (float?)null;
        }

        /// <summary>
        /// Build the network model <see cref="ServerRewardPropertySet.RewardProperty"/> for this <see cref="RewardProperty"/>.
        /// </summary>
        public IEnumerable<ServerRewardPropertySet.RewardProperty> Build()
        {
            return values.Select(p => new ServerRewardPropertySet.RewardProperty
            {
                Id    = (RewardPropertyType)Entry.Id,
                Type  = (RewardPropertyModifierValueType)Entry.RewardModifierValueTypeEnum,
                Data  = p.Key,
                Value = p.Value
            });
        }
    }
}
