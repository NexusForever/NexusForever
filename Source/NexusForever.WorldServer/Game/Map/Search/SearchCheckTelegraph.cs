using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Spell;

namespace NexusForever.WorldServer.Game.Map.Search
{
    public class SearchCheckTelegraph : ISearchCheck
    {
        private readonly Telegraph telegraph;
        private readonly UnitEntity caster;

        public SearchCheckTelegraph(Telegraph telegraph, UnitEntity caster)
        {
            this.telegraph = telegraph;
            this.caster    = caster;
        }

        public bool CheckEntity(GridEntity entity)
        {
            if (entity is not UnitEntity unit)
                return false;

            if (entity == caster)
                return false;

            return telegraph.InsideTelegraph(entity.Position, unit.HitRadius);
        }
    }
}
