using NexusForever.Game.Abstract.Achievement;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Achievement;
using NexusForever.Game.Static.Achievement;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Achievement, "A collection of commands to manage player achievements.", "achievement")]
    [CommandTarget(typeof(IPlayer))]
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
            IPlayer player = context.GetTargetOrInvoker<IPlayer>();
            player.AchievementManager.CheckAchievements(player, type, objectId, objectIdAlt, count);
        }

        [Command(Permission.AchievementGrant, "Grant achievement to player.", "grant")]
        public void HandleAchievementGrant(ICommandContext context,
            [Parameter("Achievement id to grant.")]
            ushort achievementId)
        {
            IAchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(achievementId);
            if (info == null)
            {
                context.SendMessage($"Invalid achievement id {achievementId}!");
                return;
            }

            context.GetTargetOrInvoker<IPlayer>().AchievementManager.GrantAchievement(achievementId);
        }
    }
}
