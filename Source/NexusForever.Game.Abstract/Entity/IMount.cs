using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IMount : IVehicle
    {
        uint OwnerGuid { get; }
        PetType MountType { get; }

        /// <summary>
        /// Display info applied to the pilot in the <see cref="ItemSlot.Mount"/> slot.
        /// </summary>
        ItemDisplayEntry PilotDisplayInfo { get; }
    }
}