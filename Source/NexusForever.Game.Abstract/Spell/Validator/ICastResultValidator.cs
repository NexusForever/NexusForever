using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Abstract.Spell.Validator
{
    public interface ICastResultValidator
    {
        CastResult GetCastResult(ISpell spell);
    }
}
