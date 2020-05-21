using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Item, "A collection of commands to manage items for a character.", "item")]
    [CommandTarget(typeof(Player))]
    public class ItemCommandCategory : CommandCategory
    {
        [Command(Permission.ItemAdd, "Add an item to inventory, optionally specifying quantity and charges.", "add")]
        public void HandleItemAdd(ICommandContext context,
            [Parameter("Item id to create.")]
            uint itemId,
            [Parameter("Quantity of item to create.")]
            uint? quantity,
            [Parameter("Amount of charges to add to the item.")]
            uint? charges)
        {
            quantity ??= 1u;
            charges ??= 1u;
            context.GetTargetOrInvoker<Player>().Inventory.ItemCreate(itemId, quantity.Value, ItemUpdateReason.Cheat, charges.Value);
        }
    }
}
