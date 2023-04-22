using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Account.Reward
{
    public interface IRewardProperty : INetworkBuildable<IEnumerable<ServerRewardPropertySet.RewardProperty>>
    {
        RewardPropertyEntry Entry { get; }

        /// <summary>
        /// Update the value of the supplied data value.
        /// </summary>
        /// <remarks>
        /// A positive value will increment and a negative value will decrement the value.
        /// </remarks>
        void UpdateValue(uint data, float value);

        /// <summary>
        /// Set the value of the supplied data value.
        /// </summary>
        void SetValue(uint data, float value);

        /// <summary>
        /// Get the value of the supplied data value.
        /// </summary>
        float? GetValue(uint data);
    }
}