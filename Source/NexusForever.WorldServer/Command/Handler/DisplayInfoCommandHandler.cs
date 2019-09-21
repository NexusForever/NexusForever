using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("DisplayInfo")]
    public class DisplayInfoCommandHandler : CommandCategory
    {
        public DisplayInfoCommandHandler()
            : base(true, "displayInfo")
        {
        }

        [SubCommandHandler("id", "displayInfoID - Change your look to match that of a creature.")]
        public Task DisplayInfoSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length < 1 || parameters.Length > 1)
            {
                context.SendMessageAsync("Parameters are invalid. Please try again.");
                return Task.CompletedTask;
            }
               
            if (!uint.TryParse(parameters[0], out uint displayInfo))
            {
                context.SendMessageAsync("Parameters are invalid. Please try again.");
                return Task.CompletedTask;
            }

            if (GameTableManager.Creature2.GetEntry(displayInfo) == null)
            {
                context.SendMessageAsync("Invalid displayInfo. Please try again.");
                return Task.CompletedTask;
            }

            context.Session.Player.SetDisplayInfo(displayInfo);
            return Task.CompletedTask;
        }
    }
}
