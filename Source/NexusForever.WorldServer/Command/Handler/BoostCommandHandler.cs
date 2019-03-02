using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity.Static;





namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Boost")]
    public class BoostCommandHandler : CommandCategory
    {
        public BoostCommandHandler()
            : base(true, "boost")
        {
        }

        [SubCommandHandler("level", "Boosts your character to level 50, restart client for it to take effect")]
        public Task LevelSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            //bump chracter level to 50
            context.Session.Player.Level = 50;
            return Task.CompletedTask;
        }

        [SubCommandHandler("money", "Grants 500000000 of all character currencies")]
        public Task MoneySubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            //all the currencies
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
            //all the dyes
            context.Session.GenericUnlockManager.UnlockAll(GenericUnlockType.Dye);

            //bump chracter level to 50
            context.Session.Player.Level = 50;

            //all the currencies
            for (int x = 1; x < 15; x++)
            {
                byte b = (byte)x;
                context.Session.Player.CurrencyManager.CurrencyAddAmount(b, 500000000);
            }

            return Task.CompletedTask;
        }
        

    }
}
