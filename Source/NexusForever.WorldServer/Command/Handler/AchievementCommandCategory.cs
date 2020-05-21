using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Achievement;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Achievement, "A collection of commands to manage player achievements.", "achievement")]
    [CommandTarget(typeof(Player))]
    public class AchievementCommandCategory : CommandCategory
    {
        [Command(Permission.AchievementUpdate, "Update achievement criteria for player.", "update")]
        public void HandleAchievementUpdate(ICommandContext context,
            [Parameter("Achievement criteria type to update.", ParameterFlags.None, typeof(EnumParameterConverter<AchievementType>))]
            AchievementType type,
            [Parameter("Object id to match against.")]
            uint objectId,
            [Parameter("Alternative object id to match against.")]
            uint objectIdAlt,
            [Parameter("Update count for matched criteria.")]
            uint count)
        {
            Player player = context.GetTargetOrInvoker<Player>();
            player.AchievementManager.CheckAchievements(player, type, objectId, objectIdAlt, count);
        }

        [Command(Permission.AchievementGrant, "Grant achievement to player.", "grant")]
        public void HandleAchievementGrant(ICommandContext context,
            [Parameter("Achievement id to grant.")]
            ushort achievementId)
        {
            AchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(achievementId);
            if (info == null)
            {
                context.SendMessage($"Invalid achievement id {achievementId}!");
                return;
            }

            context.GetTargetOrInvoker<Player>().AchievementManager.GrantAchievement(achievementId);
        }
    }
}
