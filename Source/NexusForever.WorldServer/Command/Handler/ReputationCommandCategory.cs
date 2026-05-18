using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Reputation;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Reputation, "A collection of commands for managing character reputations.", "rep", "reputation")]
    [CommandTarget(typeof(IPlayer))]
    public class ReputationCommandCategory : CommandCategory
    {
        [Command(Permission.ReputationUpdate, "Update the reputation for a character.", "update")]
        public void HandleReputationUpdate(ICommandContext context,
            [Parameter("Faction id to update reputation.", ParameterFlags.None, typeof(EnumParameterConverter<Faction>))]
            Faction factionId,
            [Parameter("Amount to modify the reputation.")]
            float value)
        {
            IPlayer target = context.GetTargetOrInvoker<IPlayer>();
            target.ReputationManager.UpdateReputation(factionId, value);
        }
    }
}
