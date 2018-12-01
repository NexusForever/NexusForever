using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public class ItemCommandHandler : NamedCommand
    {
        public ItemCommandHandler(ILogger<ItemCommandHandler> logger) : base("itemadd", true, logger) { }
        protected override void HandleCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.Player.Inventory.ItemCreate(uint.Parse(parameters[0]), uint.Parse(parameters[1]));
        }
    }
}
