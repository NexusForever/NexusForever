using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
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
                return Task.CompletedTask;

            var currencyId = byte.Parse(parameters[0]);
            var amount = uint.Parse(parameters[1]);

            if (currencyId > 10 || currencyId == 8 || amount > 100000000)
            {
                context.SendMessageAsync("Invalid currencyId or amount too high.");
                return Task.CompletedTask;
            }

            context.Session.Player.CurrencyManager.CurrencyAddAmount(currencyId, amount);
            context.SendMessageAsync($"Granted {amount} to currencyId {currencyId}.");
            return Task.CompletedTask;
        }

        [SubCommandHandler("list", "Lists currency IDs and names")]
        public Task ListSubCommand(CommandContext context, string command, string[] parameters)
        {
            List<string> Currencies = new List<string>
            {
                "Credits (aka Money!) - currencyId (1)",
                "Renown - currencyId (2)",
                "Elder Gems - currencyId (3)",
                "Crafting Voucher - currencyId (4)",
                "Prestige - currencyId (5)",
                "Holiday Currency: Shade's Eve - currencyId (6)",
                "Glory - currencyId (7)",
                "Holiday Currency: Winterfest - currencyId (9)",
                "Triploons - currencyId (10)",
            };

            foreach (string currencyString in Currencies)
                context.SendMessageAsync(currencyString);

            return Task.CompletedTask;
        }
    }
}