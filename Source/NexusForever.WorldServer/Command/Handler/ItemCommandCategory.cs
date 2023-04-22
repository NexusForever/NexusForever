﻿using System.Collections.Generic;
using System.Linq;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Social;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.Game.Text.Search;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Item, "A collection of commands to manage items for a character.", "item")]
    [CommandTarget(typeof(IPlayer))]
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
            context.GetTargetOrInvoker<IPlayer>().Inventory.ItemCreate(InventoryLocation.Inventory, itemId, quantity.Value, ItemUpdateReason.Cheat, charges.Value);
        }

        [Command(Permission.ItemLookup, "Lookup an item by partial name.", "lookup")]
        public void HandleItemLookup(ICommandContext context,
            [Parameter("Item name to lookup.")]
            string name,
            [Parameter("Maximum amount of results to return.")]
            int? maxResults)
        {
            List<Item2Entry> searchResults = SearchManager.Instance
                .Search<Item2Entry>(name, context.Language, e => e.LocalizedTextIdName, true)
                .Take(maxResults ?? 25)
                .ToList();

            if (searchResults.Count == 0)
            {
                context.SendMessage($"Item lookup results was 0 entries for '{name}'.");
                return;
            }

            context.SendMessage($"Item lookup results for '{name}' ({searchResults.Count}):");

            var target = context.GetTargetOrInvoker<IPlayer>();
            foreach (Item2Entry itemEntry in searchResults)
            {
                var builder = new ChatMessageBuilder
                {
                    Type = ChatChannelType.System,
                    Text = $"({itemEntry.Id}) "
                };
                builder.AppendItem(itemEntry.Id);
                target.Session.EnqueueMessageEncrypted(builder.Build());
            }
        }
    }
}
