using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckRangeAoeSelect : ISearchCheck
    {
        private readonly Vector3 vector;
        private readonly float radius;
        private readonly IGridEntity searcher;
        private readonly SpellTargetMechanicFlags targetMechanicFlags;

        public SearchCheckRangeAoeSelect(IGridEntity searcher, Vector3 position, float radius, SpellTargetMechanicFlags targetMechanicFlags)
        {
            vector                   = position;
            this.radius              = radius;
            this.searcher            = searcher;
            this.targetMechanicFlags = targetMechanicFlags;
        }

        public virtual bool CheckEntity(IGridEntity entity)
        {
            if (entity is not IUnitEntity unit)
                return false;

            // TODO: Uncomment when Combat is in
            //if (!unit.IsAlive)
            //    return false;

            if (unit is not IPlayer && targetMechanicFlags.HasFlag(SpellTargetMechanicFlags.IsPlayer))
                return false;

            // TODO: Check Angle

            // Check Target Flags
            if (unit.Faction1 == 0 && unit.Faction2 == 0) // Unable to evaluate units with no factions specified, unless this means Neutral?
                return false;

            if (targetMechanicFlags.HasFlag(SpellTargetMechanicFlags.IsEnemy))
            {
                // TODO: handle other things like "Is Immune", "Is Player and PvP Enabled"

                if ((searcher as IUnitEntity).GetDispositionTo(unit.Faction1, true) > Disposition.Neutral)
                    return false;
            }

            if (targetMechanicFlags.HasFlag(SpellTargetMechanicFlags.IsFriendly))
            {
                if ((searcher as IUnitEntity).GetDispositionTo(unit.Faction1, true) < Disposition.Neutral)
                    return false;
            } 

            if (radius > 0 && Vector3.Distance(vector, entity.Position) > radius)
                return false;

            return true;
        }
    }
}
