using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entitlement;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reward;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Entity
{
    public class CharacterEntitlementManager : EntitlementManager<ICharacterEntitlement>, ICharacterEntitlementManager
    {
        // without these the client treats the player as a trial account: trading.lua and the marketplace refuse to open,
        // level is capped at 20 and credits and renown are capped at 500,000 and 5,000
        private static readonly EntitlementType[] baseEntitlements =
        [
            EntitlementType.BaseCharacterProgressionCaps,
            EntitlementType.BaseCurrencyCaps,
            EntitlementType.EconomyParticipation,
            EntitlementType.GuildsAccess,
            EntitlementType.FullSocialParticipation,
            EntitlementType.CREDDUsage,
            EntitlementType.InGameCSAccess
        ];

        private readonly IPlayer player;

        /// <summary>
        /// Create a new <see cref="ICharacterEntitlementManager"/> from existing database model.
        /// </summary>
        /// <remarks>
        /// Any base entitlement the character doesn't already have is granted.
        /// </remarks>
        public CharacterEntitlementManager(IPlayer player, CharacterModel model)
        {
            this.player = player;

            foreach (CharacterEntitlementModel entitlementModel in model.Entitlement)
            {
                EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry(entitlementModel.EntitlementId);
                if (entry == null)
                    throw new DatabaseDataException($"Account {model.Id} has invalid entitlement {entitlementModel.EntitlementId} stored!");

                var entitlement = new CharacterEntitlement(player, entitlementModel, entry);
                entitlements.Add(entitlement.Type, entitlement);
            }

            foreach (EntitlementType type in baseEntitlements)
            {
                if (entitlements.ContainsKey(type))
                    continue;

                EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry((ulong)type);
                if (entry == null)
                    throw new InvalidOperationException($"Base entitlement {type} is missing from the Entitlement table!");

                entitlements.Add(type, new CharacterEntitlement(player, entry, 1u));
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (ICharacterEntitlement entitlement in entitlements.Values)
                entitlement.Save(context);
        }

        protected override bool CanUpdateEntitlement(EntitlementEntry entry, int value)
        {
            // only handle character entitlements in CharacterEntitlementManager
            EntitlementFlags entitlementFlags = (EntitlementFlags)entry.Flags;
            if (!entitlementFlags.HasFlag(EntitlementFlags.Character))
                return false;

            return base.CanUpdateEntitlement(entry, value);
        }

        protected override void UpdateEntitlement(EntitlementEntry entry, int value)
        {
            base.UpdateEntitlement(entry, value);

            // some reward property premium modifier entries use an existing entitlement values rather than static values
            // make sure we update these when the entitlement changes
            foreach (RewardPropertyPremiumModifierEntry modifierEntry in AssetManager.Instance.GetRewardPropertiesForTier(player.Account.AccountTier)
                .Where(e => e.EntitlementIdModifierCount == entry.Id))
            {
                player.Account.RewardPropertyManager.UpdateRewardProperty((RewardPropertyType)modifierEntry.RewardPropertyId, value, modifierEntry.RewardPropertyData);
            }
        }

        protected override ICharacterEntitlement CreateEntitlement(EntitlementEntry type)
        {
            return new CharacterEntitlement(player, type, 0);
        }

        protected override void SendEntitlement(ICharacterEntitlement entitlement)
        {
            player.Session.EnqueueMessageEncrypted(entitlement.Build());
        }
    }
}
