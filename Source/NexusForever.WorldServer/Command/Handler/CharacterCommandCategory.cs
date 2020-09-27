using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Character, "A collection of commands to manage a character.", "character")]
    [CommandTarget(typeof(Player))]
    public class CharacterCommandCategory : CommandCategory
    {
        [Command(Permission.CharacterXP, "Add XP to character.", "xp")]
        public void HandleCharacterXP(ICommandContext context,
            [Parameter("Amount of XP to grant character.")]
            uint amount)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            if (target.Level >= 50)
            {
                context.SendMessage("You must be less than max level.");
                return;
            }

            target.GrantXp(amount);
        }

        [Command(Permission.CharacterLevel, "Add level to character", "level")]
        public void HandleCharacterLevel(ICommandContext context,
            [Parameter("Level to set character.")]
            byte level)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            if (level <= target.Level || level > 50)
            {
                context.SendMessage("Level must be greater than your current level and less than max level.");
                return;
            }

            target.XpManager.SetLevel(level);
        }

        [Command(Permission.CharacterSave, "Save any pending changes to the character to the database.", "save")]
        public void HandleCharacterSave(ICommandContext context)
        {
            context.GetTargetOrInvoker<Player>().Save();
        }
    }
}
