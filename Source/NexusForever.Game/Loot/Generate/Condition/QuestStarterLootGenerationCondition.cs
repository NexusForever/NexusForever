using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate.Condition;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;

namespace NexusForever.Game.Loot.Generate.Condition
{
    public sealed class QuestStarterLootGenerationCondition(IGameTableManager gameTableManager) : ILootGenerationCondition
    {
        public bool Meets(IPlayer player, uint condition)
        {
            return CanReceive(player, condition, gameTableManager);
        }

        internal static bool CanReceive(IPlayer player, uint condition, IGameTableManager gameTableManager)
        {
            if (gameTableManager.Item.GetEntry(condition) is not { Quest2IdActivation: > 0u and <= ushort.MaxValue } item)
                return false;

            return player.QuestManager.CanStartQuest((ushort)item.Quest2IdActivation) &&
                !player.Inventory.HasItemCount(condition, 1u) &&
                !player.Inventory.Any(bag => bag.Location == InventoryLocation.PlayerBank &&
                    bag.Any(existingItem => existingItem.Id == condition));
        }
    }
}
