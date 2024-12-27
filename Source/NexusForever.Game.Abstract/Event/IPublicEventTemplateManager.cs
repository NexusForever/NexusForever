namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventTemplateManager
    {
        /// <summary>
        /// Initialise <see cref="PublicEventManager"/>.
        /// </summary>
        /// <remarks>
        /// Will load all public events from the game table and create templates for them.
        /// </remarks>
        void Initialise();

        /// <summary>
        /// Return <see cref="IPublicEventTemplate"/> with supplied id.
        /// </summary>
        IPublicEventTemplate GetTemplate(uint id);
    }
}
