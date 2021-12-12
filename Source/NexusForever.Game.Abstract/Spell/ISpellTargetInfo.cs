using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellTargetInfo
    {
        IUnitEntity Entity { get; }
        SpellEffectTargetFlags Flags { get; set; }
        float Distance { get; }
        List<ISpellTargetEffectInfo> Effects { get; }
        TargetSelectionState TargetSelectionState { get; set; }
    }
}