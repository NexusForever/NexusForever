using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Event;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Event
{
    public class PublicEventTemplateManager : IPublicEventTemplateManager
    {
        private readonly Dictionary<uint, IPublicEventTemplate> templates = [];

        #region Dependency Injection

        private readonly ILogger<PublicEventTemplateManager> log;

        private readonly IGameTableManager gameTableManager;
        private readonly IFactory<IPublicEventTemplate> templateFactory;

        public PublicEventTemplateManager(
            ILogger<PublicEventTemplateManager> log,
            IGameTableManager gameTableManager,
            IFactory<IPublicEventTemplate> templateFactory)
        {
            this.log = log;

            this.gameTableManager = gameTableManager;
            this.templateFactory = templateFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="PublicEventManager"/>.
        /// </summary>
        /// <remarks>
        /// Will load all public events from the game table and create templates for them.
        /// </remarks>
        public void Initialise()
        {
            log.LogInformation("Loading public events...");

            foreach (PublicEventEntry entry in gameTableManager.PublicEvent.Entries)
            {
                IPublicEventTemplate template = templateFactory.Resolve();
                template.Initialise(entry);
                templates.Add(entry.Id, template);
            }

            log.LogInformation("Loaded {count} public events.", templates.Count);
        }

        /// <summary>
        /// Return <see cref="IPublicEventTemplate"/> with supplied id.
        /// </summary>
        public IPublicEventTemplate GetTemplate(uint id)
        {
            return templates.TryGetValue(id, out IPublicEventTemplate template) ? template : null;
        }
    }
}
