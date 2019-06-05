using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Currency")]
    public class CurrencyCommandHandler : CommandCategory
    {
        public CurrencyCommandHandler()
            : base(true, "currency")
        {
        }

        [SubCommandHandler("add", "currencyId amount - Adds currency to character.")]
        public Task AddSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 2)
            {
                context.SendMessageAsync("Parameters are invalid. Please try again.");
                return Task.CompletedTask;
            }

            bool currencyParsed = byte.TryParse(parameters[0], out byte currencyId);
            if (!currencyParsed)
            {
                context.SendMessageAsync("Invalid currencyId. Please try again.");
                return Task.CompletedTask;
            }

            CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
            if (currencyEntry == null)
            {
                context.SendMessageAsync("Invalid currencyId. Please try again.");
                return Task.CompletedTask;
            }

            if (!uint.TryParse(parameters[1], out uint amount))
            {
                context.SendMessageAsync("Unable to parse amount. Please try again.");
                return Task.CompletedTask;
            }

            context.Session.Player.CurrencyManager.CurrencyAddAmount(currencyEntry, amount, true);
            return Task.CompletedTask;
        }

        [SubCommandHandler("list", "Lists currency IDs and names")]
        public Task ListSubCommand(CommandContext context, string command, string[] parameters)
        {
            foreach (var entry in GameTableManager.CurrencyType.Entries)
            {
                context.SendMessageAsync($"ID {entry.Id}: {entry.Description}");
            }
                
            return Task.CompletedTask;
        }
    }
}
