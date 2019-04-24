using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Items")]
    public class ItemCommandHandler : CommandCategory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ItemCommandHandler()
            : base(true, "item")
        {
        }

        [SubCommandHandler("add", "itemId [quantity] - Add an item to inventory, optionally specifying quantity")]
        public Task AddItemSubCommand(CommandContext context, string command, string[] parameters, IEnumerable<ChatFormat> chatLinks)
        {
            log.Info($"{parameters[0]}, {parameters[1]}, {chatLinks.ToList().Count}");
            List<ChatFormat> ItemLinks = chatLinks?.Where(i => (i.Type == Game.Social.Static.ChatFormatType.ItemItemId || i.Type == Game.Social.Static.ChatFormatType.ItemGuid || i.Type == Game.Social.Static.ChatFormatType.ItemFull)).ToList();
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            uint amount = 1;
            if (parameters.Length > 1)
                amount = uint.Parse(parameters[1]);

            uint itemId = 0;
            if(ItemLinks?.Count == 1)
            {
                ChatFormat itemLink = ItemLinks[0];
                if (itemLink.Type == Game.Social.Static.ChatFormatType.ItemItemId)
                {
                    ChatFormatItemId formatModel = (ChatFormatItemId)itemLink.FormatModel;
                    itemId = formatModel.ItemId;
                }
                else if (itemLink.Type == Game.Social.Static.ChatFormatType.ItemGuid)
                {
                    ChatFormatItemGuid formatModel = (ChatFormatItemGuid)itemLink.FormatModel;
                    itemId = context.Session.Player.Inventory.GetItem(formatModel.Guid).Entry.Id;
                }
            }
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
        public async Task FindItemSubCommand(CommandContext context, string command, string[] parameters, IEnumerable<ChatFormat> chatLinks)
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
