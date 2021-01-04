using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Prerequisite.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Prerequisite
{
    public sealed partial class PrerequisiteManager : Singleton<PrerequisiteManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private delegate bool PrerequisiteCheckDelegate(Player player, PrerequisiteComparison comparison, uint value, uint objectId);
        private ImmutableDictionary<PrerequisiteType, PrerequisiteCheckDelegate> prerequisiteCheckHandlers;

        private PrerequisiteManager()
        {
        }

        public void Initialise()
        {
            var builder = ImmutableDictionary.CreateBuilder<PrerequisiteType, PrerequisiteCheckDelegate>();
            foreach (MethodInfo method in Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)))
            {
                PrerequisiteCheckAttribute attribute = method.GetCustomAttribute<PrerequisiteCheckAttribute>();
                if (attribute == null)
                    continue;

                PrerequisiteCheckDelegate handler = (PrerequisiteCheckDelegate)Delegate.CreateDelegate(typeof(PrerequisiteCheckDelegate), method);
                builder.Add(attribute.Type, handler);
            }

            prerequisiteCheckHandlers = builder.ToImmutable();

            log.Info($"Initialised {prerequisiteCheckHandlers.Count} prerequisite handler(s).");
        }

        /// <summary>
        /// Checks if <see cref="Player"/> meets supplied prerequisite.
        /// </summary>
        public bool Meets(Player player, uint prerequisiteId)
        {
            PrerequisiteEntry entry = GameTableManager.Instance.Prerequisite.GetEntry(prerequisiteId);
            if (entry == null)
                throw new ArgumentException();
            
            switch ((EvaluationMode)entry.Flags)
            {
                case EvaluationMode.EvaluateAND:
                    return MeetsEvaluateAnd(player, prerequisiteId, entry);
                case EvaluationMode.EvaluateOR:
                    return MeetsEvaluateOr(player, prerequisiteId, entry);
                default:
                    log.Trace($"Unhandled EvaluationMode {entry.Flags}");
                    return false;
            }
        }

        private bool MeetsEvaluateAnd(Player player, uint prerequisiteId, PrerequisiteEntry entry)
        {
            for (int i = 0; i < entry.PrerequisiteTypeId.Length; i++)
            {
                var type = (PrerequisiteType)entry.PrerequisiteTypeId[i];
                if (type == PrerequisiteType.None)
                    continue;

                PrerequisiteComparison comparison = (PrerequisiteComparison)entry.PrerequisiteComparisonId[i];
                if (!Meets(player, type, comparison, entry.Value[i], entry.ObjectId[i]))
                {
                    log.Trace($"Player {player.Name} failed prerequisite AND check ({prerequisiteId}) {type}, {comparison}, {entry.Value[i]}, {entry.ObjectId[i]}");
                    return false;
                }
            }

            return true;
        }

        private bool MeetsEvaluateOr(Player player, uint prerequisiteId, PrerequisiteEntry entry)
        {
            for (int i = 0; i < entry.PrerequisiteTypeId.Length; i++)
            {
                var type = (PrerequisiteType)entry.PrerequisiteTypeId[i];
                if (type == PrerequisiteType.None)
                    continue;

                PrerequisiteComparison comparison = (PrerequisiteComparison)entry.PrerequisiteComparisonId[i];
                if (Meets(player, type, comparison, entry.Value[i], entry.ObjectId[i]))
                    return true;
            }

            log.Trace($"Player {player.Name} failed prerequisite OR check ({prerequisiteId})");
            return false;
        }

        private bool Meets(Player player, PrerequisiteType type, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            if (!prerequisiteCheckHandlers.TryGetValue(type, out PrerequisiteCheckDelegate handler))
            {
                log.Warn($"Unhandled PrerequisiteType {type}!");
                return false;
            }

            return handler.Invoke(player, comparison, value, objectId);
        }
    }
}
