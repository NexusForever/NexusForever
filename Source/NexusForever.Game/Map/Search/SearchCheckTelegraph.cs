using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;

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
            if (telegraph.TelegraphTargetTypeFlags.HasFlag(TelegraphTargetTypeFlags.Self) && entity != caster)
                return false;

            if (telegraph.TelegraphTargetTypeFlags.HasFlag(TelegraphTargetTypeFlags.Other) && entity == caster)
                return false;

            if (entity is not IUnitEntity unit)
                return false;

            return telegraph.InsideTelegraph(entity.Position, unit.HitRadius);
        }
    }
}
