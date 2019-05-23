using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Generic", Permission.None)]
    public class GenericUnlockCommandHandler : CommandCategory
    {
        public GenericUnlockCommandHandler()
            : base(true, "generic")
        {
        }

        [SubCommandHandler("unlock", "genericUnlockEntryId - Unlock generic unlock entry.", Permission.CommandGenericUnlock)]
        public Task UnlockSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1)
                return Task.CompletedTask;

            context.Session.GenericUnlockManager.Unlock(ushort.Parse(parameters[0]));
            return Task.CompletedTask;
        }

        [SubCommandHandler("unlockall", "genericUnlockType - Unlock all generic unlocks of type.", Permission.CommandGenericUnlockAll)]
        public Task UnlockAllSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1)
                return Task.CompletedTask;

            context.Session.GenericUnlockManager.UnlockAll((GenericUnlockType)uint.Parse(parameters[0]));
            return Task.CompletedTask;
        }

        [SubCommandHandler("list", "List all acquired generic unlock entries.", Permission.CommandGenericList)]
        public async Task ListUnlocksSubCommand(CommandContext context, string command, string[] parameters)
        {
            await context.SendMessageAsync("Acquired generic unlock entries:");

            TextTable tt = GameTableManager.GetTextTable(context.Language);
            foreach (GenericUnlock unlock in context.Session.GenericUnlockManager)
            {
                string name = tt.GetEntry(unlock.Entry.LocalizedTextIdDescription);
                await context.SendMessageAsync($"{unlock.Entry.Id}, {unlock.Type}, {name}");
            }
        }
    }
}
