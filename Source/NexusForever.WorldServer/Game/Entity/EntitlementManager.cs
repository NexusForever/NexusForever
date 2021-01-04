using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class EntitlementManager : ISaveAuth, ISaveCharacter
    {
        private readonly WorldSession session;

        private readonly Dictionary<EntitlementType, AccountEntitlement> accountEntitlements
            = new Dictionary<EntitlementType, AccountEntitlement>();
        private readonly Dictionary<EntitlementType, CharacterEntitlement> characterEntitlements
            = new Dictionary<EntitlementType, CharacterEntitlement>();

        private readonly Dictionary<RewardPropertyType, RewardProperty> rewardProperties
            = new Dictionary<RewardPropertyType, RewardProperty>();

        /// <summary>
        /// Create a new <see cref="EntitlementManager"/> from existing database model.
        /// </summary>
        public EntitlementManager(WorldSession session, AccountModel model)
        {
            this.session = session;

            foreach (AccountEntitlementModel entitlementModel in model.AccountEntitlement)
            {
                EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry(entitlementModel.EntitlementId);
                if (entry == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid entitlement {entitlementModel.EntitlementId} stored!");

                var entitlement = new AccountEntitlement(entitlementModel, entry);
                accountEntitlements.Add(entitlement.Type, entitlement);
            }

            UpdateRewardPropertiesPremiumModifiers(false);
        }

        public void Save(AuthContext context)
        {
            foreach (AccountEntitlement entitlement in accountEntitlements.Values)
                entitlement.Save(context);
        }

        public void Save(CharacterContext context)
        {
            foreach (CharacterEntitlement entitlement in characterEntitlements.Values)
                entitlement.Save(context);
        }

        public IEnumerable<AccountEntitlement> GetAccountEntitlements()
        {
            return accountEntitlements.Values;
        }

        public IEnumerable<CharacterEntitlement> GetCharacterEntitlements()
        {
            return characterEntitlements.Values;
        }

        /// <summary>
        /// Return <see cref="AccountEntitlement"/> for supplied <see cref="EntitlementType"/>.
        /// </summary>
        public AccountEntitlement GetAccountEntitlement(EntitlementType type)
        {
            return accountEntitlements.TryGetValue(type, out AccountEntitlement entitlement) ? entitlement : null;
        }

        /// <summary>
        /// Return <see cref="CharacterEntitlement"/> for supplied <see cref="EntitlementType"/>.
        /// </summary>
        public CharacterEntitlement GetCharacterEntitlement(EntitlementType type)
        {
            return characterEntitlements.TryGetValue(type, out CharacterEntitlement entitlement) ? entitlement : null;
        }

        /// <summary>
        /// Initialise entitlements and reward properties from an existing <see cref="CharacterModel"/>.
        /// </summary>
        public void Initialise(CharacterModel model)
        {
            InitialiseEntitlements(model);
            InitialiseRewardProperties(model);
        }

        private void InitialiseEntitlements(CharacterModel model)
        {
            characterEntitlements.Clear();
            foreach (CharacterEntitlementModel entitlementModel in model.Entitlement)
            {
                EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry(entitlementModel.EntitlementId);
                if (entry == null)
                    throw new DatabaseDataException($"Character {model.Id} has invalid entitlement {entitlementModel.EntitlementId} stored!");

                var entitlement = new CharacterEntitlement(entitlementModel, entry);
                characterEntitlements.Add(entitlement.Type, entitlement);
            }
        }

        private void InitialiseRewardProperties(CharacterModel model)
        {
            rewardProperties.Clear();

            // TODO: load from DB? Might be useful for custom
            UpdateRewardPropertiesPremiumModifiers(true);
        }

        private void UpdateRewardPropertiesPremiumModifiers(bool character)
        {
            foreach (RewardPropertyPremiumModifierEntry modifierEntry in AssetManager.Instance.GetRewardPropertiesForTier(session.AccountTier))
            {
                RewardPropertyEntry entry = GameTableManager.Instance.RewardProperty.GetEntry(modifierEntry.RewardPropertyId);
                if (entry == null)
                    throw new ArgumentException();

                float value = 0f;

                // some reward property premium modifier entries use an existing entitlement values rather than static values
                if (modifierEntry.EntitlementIdModifierCount != 0u)
                {
                    // TODO: If the RewardProperty value is higher on Load that the Entitlement.
                    // Should we set the Entitlement to match? This is only necessary for things like Bank Slots (4 for Signature, 2 for Basic), Auction Slots, and Commodity Slots.
                    // Do we know if you subscribed, then unsubscribed, that you would keep those Bank Slots? Did they get greyed out and unusable?
                    value += GetAccountEntitlement((EntitlementType)modifierEntry.EntitlementIdModifierCount)?.Amount ?? 0u;
                    if (character)
                        value += GetCharacterEntitlement((EntitlementType)modifierEntry.EntitlementIdModifierCount)?.Amount ?? 0u;
                }
                else
                {
                    switch ((RewardPropertyModifierValueType)entry.RewardModifierValueTypeEnum)
                    {
                        case RewardPropertyModifierValueType.AdditiveScalar:
                            value += modifierEntry.ModifierValueFloat;
                            break;
                        case RewardPropertyModifierValueType.Discrete:
                            value += modifierEntry.ModifierValueInt;
                            break;
                        case RewardPropertyModifierValueType.MultiplicativeScalar:
                            value += modifierEntry.ModifierValueFloat;
                            break;
                    }
                }

                UpdateRewardPropertyInternal((RewardPropertyType)entry.Id, value, modifierEntry.RewardPropertyData);
            }
        }

        public void SendInitialPackets()
        {
            session.EnqueueMessageEncrypted(new ServerRewardPropertySet
            {
                Properties = rewardProperties.Values
                    .SelectMany(e => e.Build())
                    .ToList()
            });
        }

        /// <summary>
        /// Create or update account <see cref="EntitlementType"/> with supplied value.
        /// </summary>
        /// <remarks>
        /// A positive value must be supplied for new entitlements otherwise an <see cref="ArgumentException"/> will be thrown.
        /// For existing entitlements a positive value will increment and a negative value will decrement the entitlement value.
        /// </remarks>
        public void SetAccountEntitlement(EntitlementType type, int value)
        {
            EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry((ulong)type);
            if (entry == null)
                throw new ArgumentException($"Invalid entitlement type {type}!");

            AccountEntitlement entitlement = SetEntitlement(accountEntitlements, entry, value,
                () => new AccountEntitlement(session.Account.Id, entry, (uint)value));

            session.EnqueueMessageEncrypted(new ServerAccountEntitlement
            {
                Entitlement = type,
                Count       = entitlement.Amount
            });

            UpdateRewardProperty(type, value);
        }

        /// <summary>
        /// Create or update character <see cref="EntitlementType"/> with supplied value.
        /// </summary>
        /// <remarks>
        /// A positive value must be supplied for new entitlements otherwise an <see cref="ArgumentException"/> will be thrown.
        /// For existing entitlements a positive value will increment and a negative value will decrement the entitlement value.
        /// </remarks>
        public void SetCharacterEntitlement(EntitlementType type, int value)
        {
            EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry((ulong)type);
            if (entry == null)
                throw new ArgumentException($"Invalid entitlement type {type}!");

            CharacterEntitlement entitlement = SetEntitlement(characterEntitlements, entry, value,
                () => new CharacterEntitlement(session.Player.CharacterId, entry, (uint)value));

            session.EnqueueMessageEncrypted(new ServerEntitlement
            {
                Entitlement = type,
                Count       = entitlement.Amount
            });

            UpdateRewardProperty(type, value);
        }

        private static T SetEntitlement<T>(IDictionary<EntitlementType, T> collection, EntitlementEntry entry, int value, Func<T> creator)
            where T : Entitlement
        {
            if (!collection.TryGetValue((EntitlementType)entry.Id, out T entitlement))
            {
                if (value < 1)
                    throw new ArgumentException($"Failed to create entitlement {entry.Id}, {value} isn't positive!");

                if (value > entry.MaxCount)
                    throw new ArgumentException($"Failed to create entitlement {entry.Id}, {value} is larger than max value {entry.MaxCount}!");

                entitlement = creator.Invoke();
                collection.Add(entitlement.Type, entitlement);
            }
            else
            {
                if (value > 0 && entitlement.Amount + (uint)value > entry.MaxCount)
                    throw new ArgumentException($"Failed to update entitlement {entry.Id}, incrementing by {value} exceeds max value!");

                if (value < 0 && (int)entitlement.Amount + value < 0)
                    throw new ArgumentException($"Failed to update entitlement {entry.Id}, decrementing by {value} subceeds 0!");

                entitlement.Amount = (uint)((int)entitlement.Amount + value);
            }

            return entitlement;
        }

        /// <summary>
        /// Update <see cref="RewardPropertyType"/> with supplied value and data.
        /// </summary>
        /// <remarks>
        /// A positive value will increment and a negative value will decrement the value.
        /// </remarks>
        public void UpdateRewardProperty(RewardPropertyType type, float value, uint data = 0u)
        {
            RewardProperty rewardProperty = UpdateRewardPropertyInternal(type, value, data);
            session.EnqueueMessageEncrypted(new ServerRewardPropertySet
            {
                Properties = rewardProperty.Build().ToList()
            });
        }

        private void UpdateRewardProperty(EntitlementType type, int value)
        {
            // some reward property premium modifier entries use an existing entitlement values rather than static values
            // make sure we update these when the entitlement changes
            foreach (RewardPropertyPremiumModifierEntry modifierEntry in AssetManager.Instance.GetRewardPropertiesForTier(session.AccountTier)
                .Where(e => (EntitlementType)e.EntitlementIdModifierCount == type))
            {
                UpdateRewardProperty((RewardPropertyType)modifierEntry.EntitlementIdModifierCount, value, modifierEntry.RewardPropertyData);
            }
        }

        private RewardProperty UpdateRewardPropertyInternal(RewardPropertyType type, float value, uint data)
        {
            RewardPropertyEntry entry = GameTableManager.Instance.RewardProperty.GetEntry((ulong)type);
            if (entry == null)
                throw new ArgumentException();

            if (!rewardProperties.TryGetValue(type, out RewardProperty rewardProperty))
            {
                rewardProperty = new RewardProperty(entry);
                rewardProperties.Add(type, rewardProperty);
            }

            rewardProperty.UpdateValue(data, value);
            return rewardProperty;
        }

        /// <summary>
        /// Returns a <see cref="RewardProperty"/> with the supplied <see cref="RewardPropertyType"/>.
        /// </summary>
        public RewardProperty GetRewardProperty(RewardPropertyType type)
        {
            return rewardProperties.TryGetValue(type, out RewardProperty rewardProperty) ? rewardProperty : null;
        }
    }
}
