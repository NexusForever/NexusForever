using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IGlobalSpellManager
    {
        /// <summary>
        /// Id to be assigned to the next spell cast.
        /// </summary>
        uint NextCastingId { get; }

        void Initialise();

        /// <summary>
        /// Return <see cref="CastResult"/> for failed cast on supplied <see cref="Vital"/>.
        /// </summary>
        CastResult GetFailedCastResultForVital(Vital vital);
    }
}
