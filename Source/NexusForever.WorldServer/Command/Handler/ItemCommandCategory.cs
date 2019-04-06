using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Game.Social.Model;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System.Collections.Generic;
using System.Linq;

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

        [Command(Permission.ItemLookup, "Lookup an item by partial name.", "lookup")]
        public void HandleItemLookup(ICommandContext context,
            [Parameter("Item name to lookup.")]
            string name)
        {
            var searchResults = SearchManager.Instance.Search<Item2Entry>(name, context.Language, e => e.LocalizedTextIdName, true).Take(25);

            if (searchResults.Count() > 0)
            {
                context.GetTargetOrInvoker<Player>().Session.EnqueueMessageEncrypted(new Network.Message.Model.ServerChat
                {
                    Channel = ChatChannel.System,
                    Text = $"Item Lookup Results for '{name}' ({searchResults.Count()}):"
                });

                foreach (Item2Entry itemEntry in searchResults)
                {
                    string message = $"({itemEntry.Id}) [I]";
                    context.GetTargetOrInvoker<Player>().Session.EnqueueMessageEncrypted(new Network.Message.Model.ServerChat
                    {
                        Channel = ChatChannel.System,
                        Text = message,
                        Formats = new List<ChatFormat>
                    {
                        new ChatFormat
                        {
                            Type        = ChatFormatType.ItemItemId,
                            StartIndex  = (ushort)(message.Length - 3),
                            StopIndex   = (ushort)message.Length,
                            FormatModel = new ChatFormatItemId
                            {
                                ItemId = itemEntry.Id
                            }
                        }
                    }
                    });
                }
            }
            else
            {
                context.GetTargetOrInvoker<Player>().Session.EnqueueMessageEncrypted(new Network.Message.Model.ServerChat
                {
                    Channel = ChatChannel.System,
                    Text = $"Item Lookup Results was 0 entries for '{name}'."
                });
            }
        }
    }
}
