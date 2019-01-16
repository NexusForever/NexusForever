using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Spell.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Spell
{
    public static class GlobalSpellManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Id to be assigned to the next spell cast.
        /// </summary>
        public static uint NextCastingId => nextCastingId++;

        /// <summary>
        /// Id to be assigned to the next spell effect.
        /// </summary>
        public static uint NextEffectId => nextEffectId++;

        private static uint nextCastingId = 1;
        private static uint nextEffectId = 1;

        private static Dictionary<uint, SpellBaseInfo> spellBaseInfoStore = new Dictionary<uint, SpellBaseInfo>();
        private static readonly Dictionary<SpellEffectType, SpellEffectDelegate> spellEffectDelegates =  new Dictionary<SpellEffectType, SpellEffectDelegate>();

        public static void Initialise()
        {
            //InitialiseSpellInfo();
            InitialiseSpellEffectHandlers();
        }

        private static void InitialiseSpellInfo()
        {
            Stopwatch sw = Stopwatch.StartNew();
            log.Info("Generating spell info...");

            var concurrentStore = new ConcurrentDictionary<uint, SpellBaseInfo>();

            Parallel.ForEach(GameTableManager.Spell4Base.Entries,
                e => { concurrentStore.TryAdd(e.Id, new SpellBaseInfo(e)); });
            spellBaseInfoStore = concurrentStore.ToDictionary(e => e.Key, e => e.Value);

            log.Info($"Cached {spellBaseInfoStore.Count} spells in {sw.ElapsedMilliseconds}ms.");
        }

        private static void InitialiseSpellEffectHandlers()
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

        /// <summary>
        /// Return <see cref="SpellBaseInfo"/>, if not already cached it will be generated before being returned.
        /// </summary>
        public static SpellBaseInfo GetSpellBaseInfo(uint spell4BaseId)
        {
            Spell4BaseEntry spell4BaseEntry = GameTableManager.Spell4Base.GetEntry(spell4BaseId);
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
        public static SpellEffectDelegate GetEffectHandler(SpellEffectType spellEffectType)
        {
            return spellEffectDelegates.TryGetValue(spellEffectType, out SpellEffectDelegate handler) ? handler : null;
        }
    }
}
