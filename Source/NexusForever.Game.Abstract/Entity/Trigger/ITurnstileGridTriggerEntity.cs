namespace NexusForever.Game.Abstract.Entity.Trigger
{
    public interface ITurnstileGridTriggerEntity : IGridTriggerEntity
    {
        /// <summary>
        /// Initialise turnstile trigger with supplied id, range and objective object id.
        /// </summary>
        void Initialise(uint id, float range, uint objectId);
    }
}
