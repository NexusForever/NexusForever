using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Abstract.Spell;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckTelegraph : ISearchCheck
    {
        private readonly ITelegraph telegraph;
        private readonly IUnitEntity caster;

        public SearchCheckTelegraph(ITelegraph telegraph, IUnitEntity caster)
        {
            this.telegraph = telegraph;
            this.caster    = caster;
        }

        public bool CheckEntity(IGridEntity entity)
        {
            if (entity is not IUnitEntity unit)
                return false;

            if (entity == caster)
                return false;

            return telegraph.InsideTelegraph(entity.Position, unit.HitRadius);
        }
    }
}
