using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IMountEntity : IVehicleEntity
    {
        uint OwnerGuid { get; }
        PetType MountType { get; }

        /// <summary>
        /// Display info applied to the pilot in the <see cref="ItemSlot.Mount"/> slot.
        /// </summary>
        ItemDisplayEntry PilotDisplayInfo { get; }

        void Initialise(IPlayer owner, uint spell4Id, uint creatureId, uint vehicleId, uint itemDisplayId);
    }
}
