using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Account.Reward
{
    public interface IRewardPropertyManager
    {
        void Initialise(IPlayer player);

        void SendInitialPackets();

        /// <summary>
        /// Returns a <see cref="IRewardProperty"/> with the supplied <see cref="RewardPropertyType"/>.
        /// </summary>
        IRewardProperty GetRewardProperty(RewardPropertyType type);

        /// <summary>
        /// Update <see cref="RewardPropertyType"/> with supplied value and data.
        /// </summary>
        /// <remarks>
        /// A positive value will increment and a negative value will decrement the value.
        /// </remarks>
        void UpdateRewardProperty(RewardPropertyType type, float value, uint data = 0);

        /// <summary>
        /// Update <see cref="RewardPropertyEntry"/> with supplied value and data.
        /// </summary>
        /// <remarks>
        /// A positive value will increment and a negative value will decrement the value.
        /// </remarks>
        void UpdateRewardProperty(RewardPropertyEntry entry, float value, uint data = 0);
    }
}