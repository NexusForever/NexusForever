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
using NexusForever.WorldServer.Game.Static;
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

        /// <summary>
        /// All <see cref="RewardProperty"/> that are sent to the Player.
        /// </summary>
        /// <remarks>This is keyed by <see cref="RewardPropertyType"/> and Data key. This Data key is usually a "type", e.g. (RewardPropertyType.AccountCurrency, 15) would be Crimson Essence. This is used to determine bonuses that are specific to a type of RewardProperty.</remarks>
        private readonly Dictionary<(RewardPropertyType, uint), RewardProperty> rewardProperties = new Dictionary<(RewardPropertyType, uint), RewardProperty>();

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

        public void OnNewCharacter(CharacterModel model)
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

            rewardProperties.Clear();
            CalculateRewardProperties();
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

            UpdateRewardProperties(type, value);
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

            UpdateRewardProperties(type, value);
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
        /// Calculate the <see cref="RewardProperty"/> Values based on <see cref="AccountTier"/>.
        /// </summary>
        private void CalculateRewardProperties()
        {
            foreach (RewardPropertyPremiumModifierEntry entry in AssetManager.Instance.GetRewardPropertiesForTier(session.AccountTier))
            {
                if (!rewardProperties.ContainsKey(((RewardPropertyType)entry.RewardPropertyId, entry.RewardPropertyData)))
                    rewardProperties.TryAdd(((RewardPropertyType)entry.RewardPropertyId, entry.RewardPropertyData), new RewardProperty(entry));

                if (rewardProperties.TryGetValue(((RewardPropertyType)entry.RewardPropertyId, entry.RewardPropertyData), out RewardProperty rewardProperty))
                    rewardProperty.AddValue(entry, this);
            }
        }

        /// <summary>
        /// This updates the <see cref="Player"/> <see cref="RewardProperty"/> with the value change.
        /// </summary>
        private void UpdateRewardProperties(EntitlementType type, int value)
        {
            if (session.Player == null)
                return;

            foreach (RewardPropertyPremiumModifierEntry rewardPropertyEntry in GameTableManager.Instance.RewardPropertyPremiumModifier.Entries.Where(i => i.Tier <= (uint)session.AccountTier && (EntitlementType)i.EntitlementIdModifierCount == type))
            {
                if (!rewardProperties.TryGetValue(((RewardPropertyType)rewardPropertyEntry.RewardPropertyId, rewardPropertyEntry.RewardPropertyData), out RewardProperty rewardProperty))
                    throw new InvalidOperationException($"Trying to modify RewardProperty from EntitlementManager but RewardProperty doesn't exist.");

                rewardProperty.AddValue(value, session.Player);
            }
        }

        /// <summary>
        /// Returns a <see cref="RewardProperty"/> that matches the <see cref="RewardPropertyType"/> and Data value.
        /// </summary>
        public RewardProperty GetRewardProperty(RewardPropertyType type, uint data)
        {
            return rewardProperties.TryGetValue((type, data), out RewardProperty rewardProperty) ? rewardProperty : null;
        }

        /// <summary>
        /// Returns a <see cref="List{T}"/> containing all the <see cref="RewardProperty"/> network messages for the Client.
        /// </summary>
        public List<ServerRewardPropertySet.RewardProperty> GetRewardPropertiesNetworkMessage()
        {
            return rewardProperties.Values.Select(e => e.Build()).ToList();
        }
    }
}
