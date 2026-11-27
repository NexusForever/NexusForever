using System.Numerics;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.Network.World.Message.Static;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Seasonal.ShadeEve
{
    [ScriptFilterCreatureId(65406, 65407)]
    public sealed class ShadeEveStarterScript(
        ISeasonalEventManager seasonalEvents,
        IGameTableManager gameTables,
        IItemManager itemManager) : IWorldEntityScript, IOwnedScript<ICreatureEntity>
    {
        private const float InteractionRangeSquared = 36f;
        private const float MarkerToleranceSquared = 1f;

        private ICreatureEntity contact;

        public void OnLoad(ICreatureEntity owner)
        {
            contact = owner;
        }

        public void OnPlayerInteract(IPlayer player)
        {
            if (contact is not INonPlayerEntity { EntityId: > 0u, IsAlive: true } ||
                !player.InWorld || player.IsLoading || !player.IsAlive || !ReferenceEquals(player.Map, contact.Map) ||
                Vector3.DistanceSquared(player.Position, contact.Position) > InteractionRangeSquared)
                return;

            (ushort questId, uint itemId, uint markerId) = (contact.CreatureId, player.Faction1) switch
            {
                (65407u, Faction.Exile) => ((ushort)9760, 49483u, 47315u),
                (65406u, Faction.Dominion) => ((ushort)9761, 49484u, 47289u),
                _ => default
            };
            if (questId == 0u || !seasonalEvents.IsEntityInQuestEvent(contact.EntityId, questId) ||
                player.QuestManager.GetQuestState(questId) is not (null or QuestState.Unknown or QuestState.Mentioned or QuestState.Abandoned) ||
                !player.QuestManager.CanAcceptQuest(questId) ||
                gameTables.WorldLocation2.GetEntry(markerId) is not { } marker ||
                contact.Map?.Entry.Id != marker.WorldId || contact.Zone?.Id != marker.WorldZoneId ||
                Vector3.DistanceSquared(contact.Position, new Vector3(marker.Position0, marker.Position1, marker.Position2)) > MarkerToleranceSquared)
                return;

            IInventory inventory = player.Inventory;
            if (inventory.HasItemCount(itemId, 1u) || inventory.Any(bag =>
                    bag.Location == InventoryLocation.PlayerBank && bag.Any(item => item.Id == itemId)) ||
                itemManager.GetItemInfo(itemId) is not { } starter || starter.Entry.Quest2IdActivation != questId)
                return;

            if (inventory.IsInventoryFull(InventoryLocation.Inventory))
                player.SendGenericError(GenericError.ItemInventoryFull);
            else
                inventory.ItemCreate(InventoryLocation.Inventory, starter, 1u, ItemUpdateReason.Loot);
        }
    }
}
