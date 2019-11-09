using System;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Achievement;
using NexusForever.WorldServer.Game.Achievement.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Achievement")]
    public class AchievementCommandHandler : CommandCategory
    {
        public AchievementCommandHandler()
            : base(true, "achievement")
        {
        }

        [SubCommandHandler("update", "type objectId objectIdAlt count - Update all achievement criteria with type and object ids by count.")]
        public async Task AchievementUpdateCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 4
                || !Enum.TryParse(typeof(AchievementType), parameters[0], out object type)
                || !uint.TryParse(parameters[1], out uint objectId)
                || !uint.TryParse(parameters[2], out uint objectIdAlt)
                || !uint.TryParse(parameters[3], out uint count))
            {
                await SendHelpAsync(context);
                return;
            }

            context.Session.Player.AchievementManager.CheckAchievements(context.Session.Player, (AchievementType)type, objectId, objectIdAlt, count);
        }

        [SubCommandHandler("grant", "achievementId - Grant the specific achievement.")]
        public async Task AchievementGrantCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1
                || !ushort.TryParse(parameters[0], out ushort achievementId))
            {
                await SendHelpAsync(context);
                return;
            }

            AchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(achievementId);
            if (info == null)
            {
                await context.SendMessageAsync($"Invalid achievement id {achievementId}!");
                return;
            }

            context.Session.Player.AchievementManager.GrantAchievement(achievementId);
        }
    }
}
