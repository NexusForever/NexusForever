using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity.Static;





namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Character Boosts and Unlocks")]
    public class BoostCommandHandler : CommandCategory
    {
        public BoostCommandHandler()
            : base(true, "boost")
        {
        }

        [SubCommandHandler("level", "Boosts your character to level 50, restart client for it to take effect")]
        public Task LevelSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            // Bump chracter level to 50
            // Later: Use levelup and exp to boost rather than directly changing the Player.Level
            context.Session.Player.Level = 50;
            return Task.CompletedTask;
        }

        [SubCommandHandler("money", "Grants 500000000 of all character currencies")]
        public Task MoneySubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            // Adds to all player currencies
            // Later: Add account currency functionality
            for (int x = 1; x < 15; x++)
            {
                byte b = (byte)x;
                context.Session.Player.CurrencyManager.CurrencyAddAmount(b, 500000000);
            }
            return Task.CompletedTask;
        }

        [SubCommandHandler("all", "Level boost, currencies and unlock all dyes")]
        public Task AllSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            //Unlocks all dyes on account
            context.Session.GenericUnlockManager.UnlockAll(GenericUnlockType.Dye);

            context.Session.Player.Level = 50;

            for (int x = 1; x < 15; x++)
            {
                byte b = (byte)x;
                context.Session.Player.CurrencyManager.CurrencyAddAmount(b, 500000000);
            }

            return Task.CompletedTask;
        }
        

    }
}
