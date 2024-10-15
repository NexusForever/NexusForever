using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity.Trigger
{
    public interface IWorldLocationVolumeGridTriggerEntity : IVolumeGridTriggerEntity
    {
        public WorldLocation2Entry Entry { get; }

        /// <summary>
        /// Initialise trigger with supplied world location id and objective object id.
        /// </summary>
        void Initialise(uint worldLocationId, uint objectId);
    }
}
