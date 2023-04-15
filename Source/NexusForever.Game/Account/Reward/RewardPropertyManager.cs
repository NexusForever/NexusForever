using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Account.Reward;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Account.Reward
{
    public class RewardPropertyManager : IRewardPropertyManager
    {
        private readonly IAccount account;
        private readonly Dictionary<RewardPropertyType, IRewardProperty> rewardProperties = new();

        public RewardPropertyManager(IAccount account)
        {
            this.account = account;
            UpdateRewardPropertiesPremiumModifiers(null);
        }

        public void Initialise(IPlayer player)
        {
            UpdateRewardPropertiesPremiumModifiers(player);
        }

        private void UpdateRewardPropertiesPremiumModifiers(IPlayer player)
        {
            rewardProperties.Clear();

            foreach (RewardPropertyPremiumModifierEntry modifierEntry in AssetManager.Instance.GetRewardPropertiesForTier(account.AccountTier))
            {
                RewardPropertyEntry entry = GameTableManager.Instance.RewardProperty.GetEntry(modifierEntry.RewardPropertyId);
                if (entry == null)
                    throw new ArgumentException();

                float value = GetPremiumModiferValue(entry, modifierEntry, player);

                // update reward without sending
                IRewardProperty rewardProperty = GetRewardProperty(entry);
                rewardProperty.UpdateValue(modifierEntry.RewardPropertyData, value);
            }
        }

        private float GetPremiumModiferValue(RewardPropertyEntry entry, RewardPropertyPremiumModifierEntry modifierEntry, IPlayer player)
        {
            // some reward property premium modifier entries use an existing entitlement values rather than static values
            if (modifierEntry.EntitlementIdModifierCount != 0u)
            {
                EntitlementEntry entitlementEntry = GameTableManager.Instance.Entitlement.GetEntry(modifierEntry.EntitlementIdModifierCount);
                if (entitlementEntry == null)
                    throw new InvalidOperationException();

                // TODO: If the RewardProperty value is higher on Load that the Entitlement.
                // Should we set the Entitlement to match? This is only necessary for things like Bank Slots (4 for Signature, 2 for Basic), Auction Slots, and Commodity Slots.
                // Do we know if you subscribed, then unsubscribed, that you would keep those Bank Slots? Did they get greyed out and unusable?
                EntitlementFlags entitlementFlags = (EntitlementFlags)entitlementEntry.Flags;
                if (entitlementFlags.HasFlag(EntitlementFlags.Disabled))
                    return 0f;

                if (entitlementFlags.HasFlag(EntitlementFlags.Character))
                    return player?.EntitlementManager.GetEntitlement((EntitlementType)modifierEntry.EntitlementIdModifierCount)?.Amount ?? 0u;

                return account.EntitlementManager.GetEntitlement((EntitlementType)modifierEntry.EntitlementIdModifierCount)?.Amount ?? 0u;
            }
            
            return (RewardPropertyModifierValueType)entry.RewardModifierValueTypeEnum switch
            {
                RewardPropertyModifierValueType.AdditiveScalar       => modifierEntry.ModifierValueFloat,
                RewardPropertyModifierValueType.Discrete             => modifierEntry.ModifierValueInt,
                RewardPropertyModifierValueType.MultiplicativeScalar => modifierEntry.ModifierValueFloat,
                _ => 0f
            };
        }

        public void SendInitialPackets()
        {
            account.Session.EnqueueMessageEncrypted(new ServerRewardPropertySet
            {
                Properties = rewardProperties.Values
                    .SelectMany(e => e.Build())
                    .ToList()
            });
        }

        /// <summary>
        /// Returns a <see cref="IRewardProperty"/> with the supplied <see cref="RewardPropertyType"/>.
        /// </summary>
        public IRewardProperty GetRewardProperty(RewardPropertyType type)
        {
            return rewardProperties.TryGetValue(type, out IRewardProperty rewardProperty) ? rewardProperty : null;
        }

        /// <summary>
        /// Update <see cref="RewardPropertyType"/> with supplied value and data.
        /// </summary>
        /// <remarks>
        /// A positive value will increment and a negative value will decrement the value.
        /// </remarks>
        public void UpdateRewardProperty(RewardPropertyType type, float value, uint data = 0u)
        {
            RewardPropertyEntry entry = GameTableManager.Instance.RewardProperty.GetEntry((ulong)type);
            if (entry == null)
                throw new ArgumentException();

            UpdateRewardProperty(entry, value, data);
        }

        /// <summary>
        /// Update <see cref="RewardPropertyEntry"/> with supplied value and data.
        /// </summary>
        /// <remarks>
        /// A positive value will increment and a negative value will decrement the value.
        /// </remarks>
        public void UpdateRewardProperty(RewardPropertyEntry entry, float value, uint data = 0u)
        {
            IRewardProperty rewardProperty = GetRewardProperty(entry);
            rewardProperty.UpdateValue(data, value);

            account.Session.EnqueueMessageEncrypted(new ServerRewardPropertySet
            {
                Properties = rewardProperty.Build().ToList()
            });
        }

        /// <summary>
        /// Returns an <see cref="IRewardProperty"/> with supplied <see cref="RewardPropertyEntry"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="IRewardProperty"/> doesn't exist, a new one will be created and returned.
        /// </remarks>
        private IRewardProperty GetRewardProperty(RewardPropertyEntry entry)
        {
            RewardPropertyType type = (RewardPropertyType)entry.Id;
            if (!rewardProperties.TryGetValue(type, out IRewardProperty rewardProperty))
            {
                rewardProperty = new RewardProperty(entry);
                rewardProperties.Add(type, rewardProperty);
            }

            return rewardProperty;
        }
    }
}
