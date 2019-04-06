using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Items")]
    public class ItemCommandHandler : CommandCategory
    {
        public ItemCommandHandler()
            : base(true, "item")
        {
        }

        [SubCommandHandler("add", "itemId [quantity] - Add an item to inventory, optionally specifying quantity")]
        public Task AddItemSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            uint amount = 1;
            if (parameters.Length > 1)
                amount = uint.Parse(parameters[1]);

            uint itemId = 0;
            if (parameters[0].StartsWith("{itemId:"))
                itemId = uint.Parse(parameters[0].Replace("{itemId:", "").Replace("}", ""));
            else
                itemId = uint.Parse(parameters[0]);

            if (itemId > 0)
                context.Session.Player.Inventory.ItemCreate(itemId, amount);
            else
                context.SendMessageAsync($"Problem trying to create item: {parameters[0]}. Please try again.");
            
            return Task.CompletedTask;
        }

        private IEnumerable<uint> GetTextIds(Item2Entry entry)
        {
            Item2Entry item = GameTableManager.Item.GetEntry(entry.Id);
            if (item != null && item.LocalizedTextIdName != 0)
                yield return item.LocalizedTextIdName;
        }

        [SubCommandHandler("find", "itemName - Search for an item by name.")]
        public async Task FindItemSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                await Task.CompletedTask;

            string searchText = string.Join(" ", parameters);
            var searchResults = SearchManager.Search<Item2Entry>(searchText, context.Language, e => e.LocalizedTextIdName, true).Take(25);

            if (searchResults.Count() > 0)
            {
                context.Session.EnqueueMessageEncrypted(new Network.Message.Model.ServerChat
                {
                    Channel = ChatChannel.System,
                    Text = $"Item Lookup Results for '{searchText}' ({searchResults.Count()}):"
                });

                foreach (Item2Entry itemEntry in searchResults)
                {
                    string message = $"({itemEntry.Id}) [I]";
                    context.Session.EnqueueMessageEncrypted(new Network.Message.Model.ServerChat
                    {
                        Channel = ChatChannel.System,
                        Text = message,
                        Formats = new List<ChatFormat>
                    {
                        new ChatFormat
                        {
                            Type        = Game.Social.Static.ChatFormatType.ItemItemId,
                            StartIndex  = (ushort)(message.Length - 3),
                            StopIndex   = (ushort)message.Length,
                            FormatModel = new Game.Social.Model.ChatFormatItemId
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
                context.Session.EnqueueMessageEncrypted(new Network.Message.Model.ServerChat
                {
                    Channel = ChatChannel.System,
                    Text = $"Item Lookup Results was 0 entries for '{searchText}'."
                });
            }
            

            await Task.CompletedTask;
        }
    }
}
