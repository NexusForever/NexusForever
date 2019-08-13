using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

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

            uint charges = 1;
            if (parameters.Length > 2)
                charges = uint.Parse(parameters[2]);

            context.Session.Player.Inventory.ItemCreate(uint.Parse(parameters[0]), amount, Game.Entity.Static.ItemUpdateReason.Cheat, charges);
            return Task.CompletedTask;
        }
    }
}
