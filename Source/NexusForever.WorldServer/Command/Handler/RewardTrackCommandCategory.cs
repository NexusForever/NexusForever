using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.RewardTrack, "A collection of commands to modify reward tracks.", "rt")]
    public class RewardTrackCommandCategory : CommandCategory
    {

        [Command(Permission.RewardTrackUpdate, "Update a reward track.", "update")]
        public void HandleRewardTrackUpdate(ICommandContext context,
        [Parameter("Reward Track ID to update")]
        uint rewardTrackId,
        [Parameter("Amount of points to modify.")]
        uint points)
        {
            {

                if (!(context.GetTargetOrInvoker<Player>() == null))
                {
                    context.SendMessage("You need to a target to use this command!");
                    return;
                }

                context.GetTargetOrInvoker<Player>().Session.RewardTrackManager.AddPoints(rewardTrackId, points);
            }
        }
    }
}
