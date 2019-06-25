using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Disable")]
    public class DisableCommandHandler : CommandCategory
    {
        public DisableCommandHandler()
            : base(true, "disable")
        {
        }

        [SubCommandHandler("info", "")]
        public async Task DisableInfoCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 2
                || !byte.TryParse(parameters[0], out byte disableType)
                || !uint.TryParse(parameters[1], out uint objectId))
            {
                await SendHelpAsync(context);
                return;
            }

            string note = DisableManager.Instance.GetDisableNote((DisableType)disableType, objectId);
            if (note == null)
            {
                await context.SendMessageAsync("That combination of disable type and object id isn't disabled!");
                return;
            }

            await context.SendMessageAsync($"Type: {(DisableType)disableType}, ObjectId: {objectId}, Note: {note}");
        }

        [SubCommandHandler("reload", "")]
        public async Task DisableReloadCommandHandler(CommandContext context, string command, string[] parameters)
        {
            DisableManager.Instance.Initialise();
            await context.SendMessageAsync("Reloaded disables from database.");
        }
    }
}
