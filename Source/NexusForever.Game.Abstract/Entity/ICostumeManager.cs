using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICostumeManager : IDatabaseCharacter, IUpdate
    {
        byte? CostumeIndex { get; }
        byte CostumeCap { get; }

        /// <summary>
        /// Return <see cref="ICostume"/> at supplied index.
        /// </summary>
        ICostume GetCostume(byte index);

        /// <summary>
        /// Return <see cref="IItemVisual"/> for <see cref="ICostume"/> at suppled index and <see cref="ItemSlot"/>.
        /// </summary>
        IItemVisual GetItemVisual(byte costumeIndex, ItemSlot slot);

        /// <summary>
        /// Return a collection of <see cref="IItemVisual"/> for <see cref="ICostume"/> at supplied index.
        /// </summary>
        IEnumerable<IItemVisual> GetItemVisuals(byte costumeIndex);

        /// <summary>
        /// Validate then save or update <see cref="ICostume"/> from <see cref="ClientCostumeSave"/> packet.
        /// </summary>
        void SaveCostume(ClientCostumeSave costumeSave);

        /// <summary>
        /// Equip <see cref="ICostume"/> at supplied index.
        /// </summary>
        void SetCostume(int index);

        void SendInitialPackets();
    }
}