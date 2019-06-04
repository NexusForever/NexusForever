using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Spell.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Spell
{
    public sealed class GlobalSpellManager : Singleton<GlobalSpellManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Id to be assigned to the next spell cast.
        /// </summary>
        public uint NextCastingId => nextCastingId++;

        /// <summary>
        /// Id to be assigned to the next spell effect.
        /// </summary>
        public uint NextEffectId => nextEffectId++;

        private uint nextCastingId = 1;
        private uint nextEffectId = 1;

        private readonly Dictionary<uint, SpellBaseInfo> spellBaseInfoStore = new Dictionary<uint, SpellBaseInfo>();
        private readonly Dictionary<SpellEffectType, SpellEffectDelegate> spellEffectDelegates =  new Dictionary<SpellEffectType, SpellEffectDelegate>();
        private readonly Dictionary<CastMethod, CastMethodDelegate> castMethodDelegates = new Dictionary<CastMethod, CastMethodDelegate>();

        // entry caches
        private ImmutableDictionary<uint, ImmutableList<Spell4Entry>> spellEntries;
        private ImmutableDictionary<uint, ImmutableList<Spell4EffectsEntry>> spellEffectEntries;
        private ImmutableDictionary<uint, ImmutableList<TelegraphDamageEntry>> spellTelegraphEntries;
        private ImmutableDictionary<uint, ImmutableList<Spell4ThresholdsEntry>> spellThresholdEntries;
        private ImmutableDictionary<uint, ImmutableList<SpellPhaseEntry>> spellPhaseEntries;

        private GlobalSpellManager()
        {
        }

        public void Initialise()
        {
            CacheSpellEntries();
            InitialiseSpellInfo();
            InitialiseSpellEffectHandlers();
            InitialiseCastMethodHandlers();
        }

        private void CacheSpellEntries()
        {
            // caching is required as most of the spell tables have 50k+ entries, calculating for each spell is SLOW
            spellEntries = GameTableManager.Instance.Spell4.Entries
                .GroupBy(e => e.Spell4BaseIdBaseSpell)
                .ToImmutableDictionary(g => g.Key, g => g
                    .OrderByDescending(e => e.TierIndex)
                    .ToImmutableList());

            spellEffectEntries = GameTableManager.Instance.Spell4Effects.Entries
                .GroupBy(e => e.SpellId)
                .ToImmutableDictionary(g => g.Key, g => g
                    .OrderBy(e => e.OrderIndex)
                    .ToImmutableList());

            spellTelegraphEntries = GameTableManager.Instance.Spell4Telegraph.Entries
                .GroupBy(e => e.Spell4Id)
                .ToImmutableDictionary(g => g.Key, g => g
                    .Select(e => GameTableManager.Instance.TelegraphDamage.GetEntry(e.TelegraphDamageId))
                    .ToImmutableList());

            spellThresholdEntries = GameTableManager.Instance.Spell4Thresholds.Entries
                .GroupBy(e => e.Spell4IdParent)
                .ToImmutableDictionary(g => g.Key, g => g
                    .OrderBy(e => e.OrderIndex)
                    .ToImmutableList());

            spellPhaseEntries = GameTableManager.Instance.SpellPhase.Entries
                .GroupBy(e => e.Spell4IdOwner)
                .ToImmutableDictionary(g => g.Key, g => g
                    .OrderBy(e => e.OrderIndex)
                    .ToImmutableList());
        }

        private void InitialiseSpellInfo()
        {
            Stopwatch sw = Stopwatch.StartNew();
            log.Info("Generating spell info...");

            foreach (Spell4BaseEntry entry in GameTableManager.Instance.Spell4Base.Entries)
                spellBaseInfoStore.Add(entry.Id, new SpellBaseInfo(entry));

            log.Info($"Cached {spellBaseInfoStore.Count} spells in {sw.ElapsedMilliseconds}ms.");
        }

        private void InitialiseSpellEffectHandlers()
        {
            foreach (MethodInfo method in Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)))
            {
                SpellEffectHandlerAttribute attribute = method.GetCustomAttribute<SpellEffectHandlerAttribute>();
                if (attribute == null)
                    continue;

                ParameterExpression spellParameter  = Expression.Parameter(typeof(Spell));
                ParameterExpression targetParameter = Expression.Parameter(typeof(UnitEntity));
                ParameterExpression effectParameter = Expression.Parameter(typeof(SpellTargetInfo.SpellTargetEffectInfo));

                MethodCallExpression call = Expression.Call(spellParameter, method, targetParameter, effectParameter);

                Expression<SpellEffectDelegate> lambda =
                    Expression.Lambda<SpellEffectDelegate>(call, spellParameter, targetParameter, effectParameter);

                spellEffectDelegates.Add(attribute.SpellEffectType, lambda.Compile());
            }
        }

        private void InitialiseCastMethodHandlers()
        {
            foreach (MethodInfo method in Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)))
            {
                List<CastMethodHandlerAttribute> attributes = method.GetCustomAttributes<CastMethodHandlerAttribute>().ToList();
                if (attributes.Count() == 0)
                    continue;
                
                foreach(CastMethodHandlerAttribute attribute in attributes)
                {
                    ParameterExpression spellParameter = Expression.Parameter(typeof(Spell));

                    MethodCallExpression call = Expression.Call(spellParameter, method);

                    Expression<CastMethodDelegate> lambda =
                        Expression.Lambda<CastMethodDelegate>(call, spellParameter);

                    castMethodDelegates.Add(attribute.CastMethod, lambda.Compile());
                }
            }
        }

        /// <summary>
        /// Return all <see cref="Spell4Entry"/>'s for the supplied spell base id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="SpellBaseInfo"/>.
        /// </remarks>
        public IEnumerable<Spell4Entry> GetSpell4Entries(uint spell4BaseId)
        {
            return spellEntries.TryGetValue(spell4BaseId, out ImmutableList<Spell4Entry> entries)
                ? entries : Enumerable.Empty<Spell4Entry>();
        }

        /// <summary>
        /// Return all <see cref="Spell4EffectsEntry"/>'s for the supplied spell id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="SpellBaseInfo"/>.
        /// </remarks>
        public IEnumerable<Spell4EffectsEntry> GetSpell4EffectEntries(uint spell4Id)
        {
            return spellEffectEntries.TryGetValue(spell4Id, out ImmutableList<Spell4EffectsEntry> entries)
                ? entries : Enumerable.Empty<Spell4EffectsEntry>();
        }

        /// <summary>
        /// Return all <see cref="TelegraphDamageEntry"/>'s for the supplied spell id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="SpellBaseInfo"/>.
        /// </remarks>
        public IEnumerable<TelegraphDamageEntry> GetTelegraphDamageEntries(uint spell4Id)
        {
            return spellTelegraphEntries.TryGetValue(spell4Id, out ImmutableList<TelegraphDamageEntry> entries)
                ? entries : Enumerable.Empty<TelegraphDamageEntry>();
        }

        /// <summary>
        /// Return all <see cref="Spell4ThresholdsEntry"/>'s for the supplied spell id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="SpellBaseInfo"/>.
        /// </remarks>
        public IEnumerable<Spell4ThresholdsEntry> GetSpell4ThresholdEntries(uint spell4Id)
        {
            return spellThresholdEntries.TryGetValue(spell4Id, out ImmutableList<Spell4ThresholdsEntry> entries)
                ? entries : Enumerable.Empty<Spell4ThresholdsEntry>();
        }

        /// <summary>
        /// Return all <see cref="SpellPhaseEntry"/>'s for the supplied spell id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="SpellBaseInfo"/>.
        /// </remarks>
        public IEnumerable<SpellPhaseEntry> GetSpellPhaseEntries(uint spell4Id)
        {
            return spellPhaseEntries.TryGetValue(spell4Id, out ImmutableList<SpellPhaseEntry> entries)
                ? entries : Enumerable.Empty<SpellPhaseEntry>();
        }

        /// <summary>
        /// Return <see cref="SpellBaseInfo"/>, if not already cached it will be generated before being returned.
        /// </summary>
        public SpellBaseInfo GetSpellBaseInfo(uint spell4BaseId)
        {
            Spell4BaseEntry spell4BaseEntry = GameTableManager.Instance.Spell4Base.GetEntry(spell4BaseId);
            if (spell4BaseEntry == null)
                throw new ArgumentOutOfRangeException();

            if (!spellBaseInfoStore.TryGetValue(spell4BaseId, out SpellBaseInfo spellBaseInfo))
            {
                spellBaseInfo = new SpellBaseInfo(spell4BaseEntry);
                spellBaseInfoStore.Add(spell4BaseId, spellBaseInfo);
            }
            
            return spellBaseInfo;
        }

        /// <summary>
        /// Return <see cref="SpellEffectDelegate"/> for supplied <see cref="SpellEffectType"/>.
        /// </summary>
        public SpellEffectDelegate GetEffectHandler(SpellEffectType spellEffectType)
        {
            return spellEffectDelegates.TryGetValue(spellEffectType, out SpellEffectDelegate handler) ? handler : null;
        }

        /// <summary>
        /// Return <see cref="CastMethodDelegate"/> for supplied <see cref="SpellEffectType"/>.
        /// </summary>
        public CastMethodDelegate GetCastMethodHandler(CastMethod castMethod)
        {
            return castMethodDelegates.TryGetValue(castMethod, out CastMethodDelegate handler) ? handler : null;
        }
    }
}
