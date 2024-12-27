using NexusForever.Game.Abstract.Event;
using NexusForever.Shared;

namespace NexusForever.Game.Event
{
    public class PublicEventFactory : IPublicEventFactory
    {
        #region Dependency Injection

        private readonly IPublicEventTemplateManager publicEventManager;
        private readonly IFactory<IPublicEvent> publicEventFactory;

        public PublicEventFactory(
            IPublicEventTemplateManager publicEventManager,
            IFactory<IPublicEvent> publicEventFactory)
        {
            this.publicEventManager = publicEventManager;
            this.publicEventFactory = publicEventFactory;
        }

        #endregion

        /// <summary>
        /// Create a new <see cref="IPublicEvent"/> with the supplied id.
        /// </summary>
        public IPublicEvent CreateEvent(uint id)
        {
            IPublicEventTemplate template = publicEventManager.GetTemplate(id);
            if (template == null)
                throw new ArgumentOutOfRangeException();

            return publicEventFactory.Resolve();
        }
    }
}
