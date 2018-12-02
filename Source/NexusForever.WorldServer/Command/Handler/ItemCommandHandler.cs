using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Items")]
    public class ItemCommandHandler : CommandCategory
    {
        public ItemCommandHandler(ILogger<ItemCommandHandler> logger)
            : base("item", true, logger) { }

        [SubCommandHandler("add", "itemId [quantity] - Add an item to inventory, optionally specifying quantity")]
        public void AddItemSubCommand(CommandContext context, string command, string[] parameters)
        {
            if(parameters.Length <= 0)
                return;

            uint amount = 1;
            if(parameters.Length > 1)
                amount = uint.Parse(parameters[1]);

            context.Session.Player.Inventory.ItemCreate(uint.Parse(parameters[0]), amount);
        }
    }
}
