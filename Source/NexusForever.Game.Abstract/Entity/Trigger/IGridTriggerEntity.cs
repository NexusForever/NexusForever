namespace NexusForever.Game.Abstract.Entity.Trigger
{
    public interface IGridTriggerEntity : IGridEntity
    {
        /// <summary>
        /// Initialise trigger with supplied id and range.
        /// </summary>
        void Initialise(uint id, float range);
    }
}
