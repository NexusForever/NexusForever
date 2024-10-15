using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Entity
{
    public class EntitySummonTemplate : IEntitySummonTemplate
    {
        public uint CreatureId { get; set; }
        public uint DisplayInfoId { get; set; }
        public Faction Faction { get; set; }
    }
}
