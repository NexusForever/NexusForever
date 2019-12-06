using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Modify")]
    public class ModifyCommandHandler : CommandCategory
    {
        public ModifyCommandHandler()
            : base(true, "modify")
        {
        }

        [SubCommandHandler("displayInfo", "id - Change your look to match that of a creature.")]
        public async Task DisplayInfoSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1)
            {
                await SendHelpAsync(context);
                return;
            }
               
            if (!uint.TryParse(parameters[0], out uint displayInfo))
            {
                await SendHelpAsync(context);
                return;
            }

            if (GameTableManager.Instance.Creature2DisplayInfo.GetEntry(displayInfo) == null)
            {
                await SendHelpAsync(context);
                return;
            }

            context.Session.Player.SetDisplayInfo(displayInfo);
        }
    }
}
