using System.Collections.Immutable;
using System.Reflection;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;

namespace NexusForever.Game.Spell
{
    public sealed class GlobalSpellManager : Singleton<GlobalSpellManager>, IGlobalSpellManager
    {
        /// <summary>
        /// Id to be assigned to the next spell cast.
        /// </summary>
        public uint NextCastingId => nextCastingId++;
        private uint nextCastingId = 1;

        private ImmutableDictionary<Vital, CastResult> vitalCastResults;

        public void Initialise()
        {
            InitialiseVitalCastResults();
        }

        private void InitialiseVitalCastResults()
        {
            var builder = ImmutableDictionary.CreateBuilder<Vital, CastResult>();

            foreach (FieldInfo field in typeof(CastResult).GetFields())
            {
                IEnumerable<CastResultVitalAttribute> attributes = field.GetCustomAttributes<CastResultVitalAttribute>();

                foreach (CastResultVitalAttribute attribute in attributes)
                    builder.Add(attribute.Vital, (CastResult)field.GetValue(null));
            }

            vitalCastResults = builder.ToImmutable();
        }

        /// <summary>
        /// Return <see cref="CastResult"/> for failed cast on supplied <see cref="Vital"/>.
        /// </summary>
        public CastResult GetFailedCastResultForVital(Vital vital)
        {
            return vitalCastResults.TryGetValue(vital, out CastResult result) ? result : CastResult.SpellBad;
        }
    }
}
