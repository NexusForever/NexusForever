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
            return entity is UnitEntity && caster != entity && telegraph.InsideTelegraph(entity.Position);
        }
    }
}
