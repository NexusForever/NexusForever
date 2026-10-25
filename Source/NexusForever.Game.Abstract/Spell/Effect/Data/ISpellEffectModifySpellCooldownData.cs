using NexusForever.Game.Static.Spell.Effect;

namespace NexusForever.Game.Abstract.Spell.Effect.Data
{
    public interface ISpellEffectModifySpellCooldownData : ISpellEffectData
    {
        SpellEffectModifyCooldownScope Scope { get; }
        uint Key { get; }
        SpellEffectModifyCooldownMode Mode { get; }
        float Magnitude { get; }
    }
}
