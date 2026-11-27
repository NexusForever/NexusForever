using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.PublicEvent;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.PublicEvent;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.PublicEventObjective188)]
    public sealed class PrerequisiteCheckPublicEventObjective : IPrerequisiteCheck
    {
        private readonly IGameTableManager gameTableManager;

        public PrerequisiteCheckPublicEventObjective(IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (value > (uint)PublicEventStatus.Failed ||
                comparison is not (PrerequisiteComparison.Equal or PrerequisiteComparison.NotEqual))
                return false;

            PublicEventObjectiveEntry entry = gameTableManager.PublicEventObjective.GetEntry(objectId);
            if (entry == null || player.Map is not { } map ||
                map.PublicEventManager?.GetEvent(entry.PublicEventId) is not { IsFinalised: false } publicEvent ||
                !ReferenceEquals(publicEvent.Map, map))
                return false;

            IEnumerable<IPublicEventTeam> teams = publicEvent.GetTeams();
            IPublicEventTeam playerTeam = teams.FirstOrDefault(team =>
                team.GetMembers().Any(member => member.CharacterId == player.CharacterId));
            IPublicEventTeam objectiveTeam = teams.FirstOrDefault(team => team.Team == entry.PublicEventTeamId);
            if (playerTeam == null || objectiveTeam == null)
                return false;

            IEnumerable<IPublicEventObjective> visibleObjectives = ReferenceEquals(playerTeam, objectiveTeam)
                ? objectiveTeam.GetObjectives()
                : objectiveTeam.GetOtherObjectives();
            IPublicEventObjective objective = visibleObjectives.FirstOrDefault(objective => objective.Entry.Id == entry.Id);
            if (objective == null)
                return false;

            bool matches = objective.Status == (PublicEventStatus)value;
            return comparison == PrerequisiteComparison.Equal ? matches : !matches;
        }
    }
}
