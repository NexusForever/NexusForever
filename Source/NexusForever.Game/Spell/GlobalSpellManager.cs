using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Spell
{
    public sealed class GlobalSpellManager : Singleton<GlobalSpellManager>, IGlobalSpellManager
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

        // Spell Delegates
        private delegate Spell SpellFactoryDelegate(IUnitEntity unit, ISpellParameters parameters);
        private ImmutableDictionary<CastMethod, SpellFactoryDelegate> spellFactoryDelegates;
        private readonly Dictionary<SpellEffectType, SpellEffectDelegate> spellEffectDelegates = new();

        private readonly Dictionary<uint, ISpellBaseInfo> spellBaseInfoStore = new();

        // entry caches
        private ImmutableDictionary<uint, ImmutableList<Spell4Entry>> spellEntries;
        private ImmutableDictionary<uint, ImmutableList<Spell4EffectsEntry>> spellEffectEntries;
        private ImmutableDictionary<uint, ImmutableList<TelegraphDamageEntry>> spellTelegraphEntries;
        private ImmutableDictionary<uint, ImmutableList<Spell4ThresholdsEntry>> spellThresholdEntries;
        private ImmutableDictionary<uint, ImmutableList<SpellPhaseEntry>> spellPhaseEntries;

        public void Initialise()
        {
            InitialiseSpellFactories();
            CacheSpellEntries();
            InitialiseSpellInfo();
            InitialiseSpellEffectHandlers();
        }

        private void InitialiseSpellFactories()
        {
            var builder = ImmutableDictionary.CreateBuilder<CastMethod, SpellFactoryDelegate>();

            Type[] types = new Type[2];
            types[0] = typeof(IUnitEntity);
            types[1] = typeof(ISpellParameters);

            ParameterExpression[] constructorParams = new ParameterExpression[2];
            constructorParams[0] = Expression.Parameter(typeof(IUnitEntity));
            constructorParams[1] = Expression.Parameter(typeof(ISpellParameters));

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var attribute in type.GetCustomAttributes<SpellTypeAttribute>())
                {
                    if (attribute == null)
                        continue;

                    ConstructorInfo constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                        null, types, null);

                    NewExpression @new = Expression.New(constructor, constructorParams);

                    Expression<SpellFactoryDelegate> lambda =
                        Expression.Lambda<SpellFactoryDelegate>(@new, constructorParams[0], constructorParams[1]);

                    builder.Add(attribute.CastMethod, lambda.Compile());
                }
            }

            spellFactoryDelegates = builder.ToImmutable();
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
                    .Where(e => e != null)
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

            foreach (SpellBaseInfo spellBaseInfo in spellBaseInfoStore.Values)
                spellBaseInfo.Intitialise();

            log.Info($"Cached {spellBaseInfoStore.Count} spells in {sw.ElapsedMilliseconds}ms.");
        }

        private void InitialiseSpellEffectHandlers()
        {
            foreach (MethodInfo method in Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods()))
            {
                SpellEffectHandlerAttribute attribute = method.GetCustomAttribute<SpellEffectHandlerAttribute>();
                if (attribute == null)
                    continue;

                ParameterExpression spellParameter  = Expression.Parameter(typeof(ISpell));
                ParameterExpression targetParameter = Expression.Parameter(typeof(IUnitEntity));
                ParameterExpression effectParameter = Expression.Parameter(typeof(ISpellTargetEffectInfo));

                MethodCallExpression call = Expression.Call(method, spellParameter, targetParameter, effectParameter);

                Expression<SpellEffectDelegate> lambda =
                    Expression.Lambda<SpellEffectDelegate>(call, spellParameter, targetParameter, effectParameter);

                spellEffectDelegates.Add(attribute.SpellEffectType, lambda.Compile());
            }
        }

        /// <summary>
        /// Return a new <see cref="IWorldEntity"/> of supplied <see cref="EntityType"/>.
        /// </summary>
        public Spell NewSpell(CastMethod castMethod, IUnitEntity caster, ISpellParameters parameters)
        {
            if (spellFactoryDelegates.TryGetValue(castMethod, out SpellFactoryDelegate factory))
                return factory.Invoke(caster, parameters);

            log.Warn($"Unhandled cast method {castMethod}. Using {CastMethod.Normal} instead.");
            if (spellFactoryDelegates.TryGetValue(CastMethod.Normal, out SpellFactoryDelegate normalFactory))
                return normalFactory.Invoke(caster, parameters);

            return null;
        }

        /// <summary>
        /// Return all <see cref="Spell4Entry"/>'s for the supplied spell base id.
        /// </summary>
        /// <remarks>
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="ISpellBaseInfo"/>.
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
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="ISpellBaseInfo"/>.
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
        /// This should only be used for cache related code, if you want an overview of a spell use <see cref="ISpellBaseInfo"/>.
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
        /// Return <see cref="ISpellBaseInfo"/>, if not already cached it will be generated before being returned.
        /// </summary>
        public ISpellBaseInfo GetSpellBaseInfo(uint spell4BaseId)
        {
            Spell4BaseEntry spell4BaseEntry = GameTableManager.Instance.Spell4Base.GetEntry(spell4BaseId);
            if (spell4BaseEntry == null)
                throw new ArgumentOutOfRangeException();

            if (!spellBaseInfoStore.TryGetValue(spell4BaseId, out ISpellBaseInfo spellBaseInfo))
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
    }
}
