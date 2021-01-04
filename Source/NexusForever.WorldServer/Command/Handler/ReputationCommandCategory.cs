using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Game.Reputation.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Reputation, "A collection of commands for managing character reputations.", "rep", "reputation")]
    [CommandTarget(typeof(Player))]
    public class ReputationCommandCategory : CommandCategory
    {
        [Command(Permission.ReputationUpdate, "Update the reputation for a character.", "update")]
        public void HandleReputationUpdate(ICommandContext context,
            [Parameter("Faction id to update reputation.", ParameterFlags.None, typeof(EnumParameterConverter<Faction>))]
            Faction factionId,
            [Parameter("Amount to modify the reputation.")]
            float value)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            target.ReputationManager.UpdateReputation(factionId, value);
        }
    }
}
