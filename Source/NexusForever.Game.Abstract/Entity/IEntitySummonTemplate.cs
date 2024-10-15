using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IEntitySummonTemplate
    {
        uint CreatureId { get; set; }
        uint DisplayInfoId { get; set; }
        Faction Faction { get; set; }
    }
}
