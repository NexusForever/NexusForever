using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICostume : IDatabaseCharacter, INetworkBuildable<Costume>, IEnumerable<ICostumeItem>
    {
        ulong Owner { get; }
        byte Index { get; }
        uint Mask { get; set; }

        /// <summary>
        /// Return <see cref="ICostumeItem"/> at supplied index.
        /// </summary>
        ICostumeItem GetItem(CostumeItemSlot slot);

        /// <summary>
        /// Get <see cref="ICostumeItem"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        ICostumeItem GetItem(ItemSlot slot);

        /// <summary>
        /// Get <see cref="IItemVisual"/> for <see cref="ICostumeItem"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        IItemVisual GetItemVisual(ItemSlot slot);

        /// <summary>
        /// Return a collection of <see cref="IItemVisual"/> for <see cref="ICostume"/>.
        /// </summary>
        IEnumerable<IItemVisual> GetItemVisuals();

        /// <summary>
        /// Update <see cref="ICostume"/> from <see cref="ClientCostumeSave"/>.
        /// </summary>
        void Update(ClientCostumeSave costumeSave);
    }
}