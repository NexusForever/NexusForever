using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterCreatureId(Dorian, Artemis)]
    public sealed class MissionsMentorScript : IEntityVisibilityScript, IOwnedScript<ICreatureEntity>
    {
        private const uint Dorian = 73491u;
        private const uint Artemis = 74861u;

        private ICreatureEntity mentor;

        public void OnLoad(ICreatureEntity owner) => mentor = owner;

        public bool CanBeSeenBy(IPlayer viewer)
        {
            if (mentor is not { Map.Entry.Id: MissionsArrivalScript.TutorialWorld })
                return true;

            Faction faction = mentor.CreatureId == Dorian ? Faction.Exile : Faction.Dominion;
            return viewer.Faction2 == faction;
        }
    }
}
