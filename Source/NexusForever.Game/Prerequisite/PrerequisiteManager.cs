using System.Collections.Immutable;
using System.Reflection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Prerequisite
{
    public sealed partial class PrerequisiteManager : Singleton<PrerequisiteManager>, IPrerequisiteManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private delegate bool PrerequisiteCheckDelegate(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId);
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
        /// Checks if <see cref="IPlayer"/> meets supplied prerequisite.
        /// </summary>
        public bool Meets(IPlayer player, uint prerequisiteId)
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

        private bool MeetsEvaluateAnd(IPlayer player, uint prerequisiteId, PrerequisiteEntry entry)
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

        private bool MeetsEvaluateOr(IPlayer player, uint prerequisiteId, PrerequisiteEntry entry)
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

        private bool Meets(IPlayer player, PrerequisiteType type, PrerequisiteComparison comparison, uint value, uint objectId)
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
