using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Abstract.Spell.Validator
{
    public interface ICastResultValidatorManager
    {
        CastResult GetCastResult(ISpell spell);
    }
}
