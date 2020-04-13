using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Character")]
    public class CharacterCommandHandler : CommandCategory
    {
        public CharacterCommandHandler()
            : base(true, "character")
        {
        }

        [SubCommandHandler("addxp", "amount - Add the amount to your total xp.")]
        public Task AddXPCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                uint xp = uint.Parse(parameters[0]);

                if (context.Session.Player.Level < 50)
                    context.Session.Player.GrantXp(xp);
                else
                    context.SendMessageAsync("You must be less than max level.");
            }
            else
                context.SendMessageAsync("You must specify the amount of XP you wish to add.");

            return Task.CompletedTask;
        }

        // TODO: Update after "SetStat" packets are available.
        [SubCommandHandler("level", "value - Set your level to the value passed in")]
        public Task SetLevelCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                byte level = byte.Parse(parameters[0]);

                if (context.Session.Player.Level < level && level <= 50)
                {
                    context.Session.Player.XpManager.SetLevel(level);
                    context.SendMessageAsync($"Success! You are now level {level}.");
                }
                else
                    context.SendMessageAsync("Level must be more than your current level and no higher than level 50.");
            }
            else
            {
                context.SendMessageAsync("You must specify the level value you wish to assign.");
            }

            return Task.CompletedTask;
        }
    }
}
