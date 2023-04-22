﻿using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
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

        private readonly Dictionary<uint, ISpellBaseInfo> spellBaseInfoStore = new();
        private readonly Dictionary<SpellEffectType, SpellEffectDelegate> spellEffectDelegates = new();

        // entry caches
        private ImmutableDictionary<uint, ImmutableList<Spell4Entry>> spellEntries;
        private ImmutableDictionary<uint, ImmutableList<Spell4EffectsEntry>> spellEffectEntries;
        private ImmutableDictionary<uint, ImmutableList<TelegraphDamageEntry>> spellTelegraphEntries;

        public void Initialise()
        {
            CacheSpellEntries();
            InitialiseSpellInfo();
            InitialiseSpellEffectHandlers();
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
