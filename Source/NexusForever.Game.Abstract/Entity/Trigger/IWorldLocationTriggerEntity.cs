namespace NexusForever.Game.Abstract.Entity.Trigger
{
    public interface IWorldLocationTriggerEntity : IGridEntity
    {
        /// <summary>
        /// Initialise trigger with supplied world location id.
        /// </summary>
        void Initialise(uint id);
    }
}
