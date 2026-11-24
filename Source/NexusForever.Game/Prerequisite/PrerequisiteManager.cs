using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Prerequisite
{
    public class PrerequisiteManager : Singleton<PrerequisiteManager>, IPrerequisiteManager
    {
        private const uint MaxChainDepth = 4;

        #region Dependency Injection

        private readonly ILogger<PrerequisiteManager> log;
        private readonly IServiceProvider serviceProvider;
        private readonly IGameTableManager gameTableManager;
        private readonly IFactory<IPrerequisiteParameters> prerequisiteParametersFactory;

        public PrerequisiteManager(
            ILogger<PrerequisiteManager> log,
            IServiceProvider serviceProvider,
            IGameTableManager gameTableManager,
            IFactory<IPrerequisiteParameters> prerequisiteParametersFactory)
        {
            this.log                           = log;
            this.serviceProvider               = serviceProvider;
            this.gameTableManager              = gameTableManager;
            this.prerequisiteParametersFactory = prerequisiteParametersFactory;
        }

        #endregion

        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets supplied prerequisite.
        /// </summary>
        public bool Meets(IPlayer player, uint prerequisiteId)
        {
            IPrerequisiteParameters parameters = prerequisiteParametersFactory.Resolve();
            return Meets(player, prerequisiteId, parameters);
        }

        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets supplied prerequisite.
        /// </summary>
        public bool Meets(IPlayer player, uint prerequisiteId, IPrerequisiteParameters parameters)
        {
            PrerequisiteEntry entry = gameTableManager.Prerequisite.GetEntry(prerequisiteId);
            if (entry == null)
                throw new ArgumentException();

            switch (entry.Flags)
            {
                case EvaluationMode.EvaluateAND:
                    return MeetsEvaluateAnd(player, prerequisiteId, entry, parameters);
                case EvaluationMode.EvaluateOR:
                    return MeetsEvaluateOr(player, prerequisiteId, entry, parameters);
                default:
                    log.LogTrace($"Unhandled EvaluationMode {entry.Flags}");
                    return false;
            }
        }

        /// <summary>
        /// Returns if the supplied prerequisite, and anything it chains to, can be evaluated.
        /// </summary>
        public bool CanEvaluate(uint prerequisiteId)
        {
            return CanEvaluate(prerequisiteId, 0u);
        }

        private bool CanEvaluate(uint prerequisiteId, uint depth)
        {
            // guards against a prerequisite chain that points back at itself
            if (depth > MaxChainDepth)
                return false;

            PrerequisiteEntry entry = gameTableManager.Prerequisite.GetEntry(prerequisiteId);
            if (entry == null)
                return false;

            for (int i = 0; i < entry.PrerequisiteTypeId.Length; i++)
            {
                PrerequisiteType type = entry.PrerequisiteTypeId[i];
                if (type == PrerequisiteType.None)
                    continue;

                if (serviceProvider.GetKeyedService<IPrerequisiteCheck>(type) == null)
                    return false;

                // OtherPrerequisite has a check, but it only defers to the prerequisite named by its
                // object id, so that one has to be evaluable too
                if (type == PrerequisiteType.OtherPrerequisite && !CanEvaluate(entry.ObjectId[i], depth + 1u))
                    return false;
            }

            return true;
        }

        private bool MeetsEvaluateAnd(IPlayer player, uint prerequisiteId, PrerequisiteEntry entry, IPrerequisiteParameters parameters)
        {
            for (int i = 0; i < entry.PrerequisiteTypeId.Length; i++)
            {
                PrerequisiteType type = entry.PrerequisiteTypeId[i];
                if (type == PrerequisiteType.None)
                    continue;

                PrerequisiteComparison comparison = entry.PrerequisiteComparisonId[i];
                if (!Meets(player, type, comparison, entry.Value[i], entry.ObjectId[i], parameters))
                {
                    log.LogTrace($"Player {player.Name} failed prerequisite AND check ({prerequisiteId}) {type}, {comparison}, {entry.Value[i]}, {entry.ObjectId[i]}");
                    return false;
                }
            }

            return true;
        }

        private bool MeetsEvaluateOr(IPlayer player, uint prerequisiteId, PrerequisiteEntry entry, IPrerequisiteParameters parameters)
        {
            for (int i = 0; i < entry.PrerequisiteTypeId.Length; i++)
            {
                PrerequisiteType type = entry.PrerequisiteTypeId[i];
                if (type == PrerequisiteType.None)
                    continue;

                if (Meets(player, type, entry.PrerequisiteComparisonId[i], entry.Value[i], entry.ObjectId[i], parameters))
                    return true;
            }

            log.LogTrace($"Player {player.Name} failed prerequisite OR check ({prerequisiteId})");
            return false;
        }

        private bool Meets(IPlayer player, PrerequisiteType type, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            IPrerequisiteCheck handler = serviceProvider.GetKeyedService<IPrerequisiteCheck>(type);
            if (handler == null)
            {
                log.LogWarning($"Unhandled PrerequisiteType {type}!");
                return false;
            }

            return handler.Meets(player, comparison, value, objectId, parameters);
        }
    }
}
