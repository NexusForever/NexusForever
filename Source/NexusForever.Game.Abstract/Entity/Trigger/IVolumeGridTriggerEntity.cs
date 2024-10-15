namespace NexusForever.Game.Abstract.Entity.Trigger
{
    public interface IVolumeGridTriggerEntity : IGridTriggerEntity
    {
        /// <summary>
        /// Initialise volume trigger with supplied id, range and objective object id.
        /// </summary>
        void Initialise(uint id, float range, uint objectId);
    }
}
