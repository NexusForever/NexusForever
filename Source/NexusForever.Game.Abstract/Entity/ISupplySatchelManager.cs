using NexusForever.Database.Character;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ISupplySatchelManager : IDatabaseCharacter, IEnumerable<ITradeskillMaterial>
    {
        ushort[] BuildNetworkPacket();

        /// <summary>
        /// Add the provided amount to the <see cref="ITradeskillMaterial"/> that is associated with the provided <see cref="IItem"/>
        /// </summary>
        /// <returns>
        /// Returns the remainder that could not be added.
        /// </returns>
        uint AddAmount(IItem item, uint amount);

        /// <summary>
        /// Add the provided amount to the <see cref="ITradeskillMaterial"/> for the given Material ID
        /// </summary>
        /// <returns>
        /// Returns the remainder that could not be added.
        /// </returns>
        uint AddAmount(ushort materialId, uint amount);

        /// <summary>
        /// Remove the provided amount to the <see cref="ITradeskillMaterial"/> that is associated with the provided <see cref="IItem"/>
        /// </summary>
        void RemoveAmount(IItem item, uint amount);

        /// <summary>
        /// Moves the given amount of the given material to the player's <see cref="IInventory"/>
        /// </summary>
        void MoveToInventory(ushort materialId, uint amount);

        /// <summary>
        /// Returns whether the <see cref="ITradeskillMaterial"/> associated with the given <see cref="IItem"/> is currently at its maximum amount
        /// </summary>
        bool IsFull(IItem item);

        /// <summary>
        /// Returns whether there is enough of the given material
        /// </summary>
        bool CanAfford(ushort materialId, uint amount);
    }
}