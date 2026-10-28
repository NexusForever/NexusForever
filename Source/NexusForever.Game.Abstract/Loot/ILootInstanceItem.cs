using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;
using NexusForever.Shared;
using NetworkLootItem = NexusForever.Network.World.Message.Model.Loot.LootItem;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootInstanceItem : IUpdate, INetworkBuildable<NetworkLootItem>
    {
        ILootInstance LootInstance { get; }

        uint Id { get; }
        uint StaticId { get; }
        LootItemType Type { get; }
        uint Count { get; }
        Identity Winner { get; }
        bool CanLoot { get; }
        bool Delivered { get; }

        void Initialise(ILootInstance lootInstance, ILootBuilderItem lootBuilderItem);

        void Initialise(ILootInstance lootInstance, uint staticId, LootItemType type, uint count, IEnumerable<Identity> looters);

        /// <summary>
        /// Determines if the specified identity is a master looter.
        /// </summary>
        /// <param name="identity">The identity to check.</param>
        /// <returns>
        /// <see langword="true"/> if the identity is a master looter, otherwise <see langword="false"/>.
        /// </returns>
        bool IsMasterLooter(Identity identity);

        /// <summary>
        /// Sets the specified identity as a master looter.
        /// </summary>
        /// <remarks>
        /// Retail appears to have supported only a single master looter. However, as the client supports multiple master looters, this implementation allows multiple.
        /// </remarks>
        /// <param name="identity">The identity to set as a master looter.</param>
        void SetMasterLooter(Identity identity);

        /// <summary>
        /// Sets the winner of the loot item.
        /// </summary>
        /// <remarks>
        /// Loot item still needs to be delivered to the winner
        /// </remarks>
        /// <param name="winner">The identity of the winning player.</param>
        void SetWinner(Identity winner);

        /// <summary>
        /// Makes the loot item available for looting.
        /// </summary>
        /// <remarks>
        /// Lootable means players can ether loot or vaccum the item from the ground. This usually occurs for Free For All loot items, or when a loot roll has completed and no winner was determined.
        /// </remarks>
        void SetLootable();

        /// <summary>
        /// Sets the time for the loot roll.
        /// </summary>
        /// <param name="timeSpan">The time span for the loot roll.</param>
        void SetRollTime(TimeSpan timeSpan);

        /// <summary>
        /// Sets the roll action for the specified identity.
        /// </summary>
        /// <param name="identity">The identity to set the roll action for.</param>
        /// <param name="lootRollAction">The loot roll action to set.</param>
        /// <remarks>
        /// If the roll action is not <see cref="LootRollAction.Pass"/>, a unique roll value will be assigned to the identity. If all looters have set their roll actions, the winner will be decided.
        /// </remarks>
        void SetRollAction(Identity identity, LootRollAction lootRollAction);

        /// <summary>
        /// Takes the loot for the specified player.
        /// </summary>
        /// <param name="player">The player taking the loot.</param>
        /// <remarks>
        /// This method can only be called when the loot has no winner, such as for Free For All loot or when a loot roll completed and no winner was determined.
        /// </remarks>
        void TakeLoot(IPlayer player);

        /// <summary>
        /// Assigns the loot to the specified player.
        /// </summary>
        /// <param name="player">The player to assign the loot to.</param>
        /// <remarks>
        /// This method can only be called when the loot is being assigned by a master looter.
        /// </remarks>
        void AssignLoot(IPlayer player);

        /// <summary>
        /// Delivers the loot to the specified player.
        /// </summary>
        /// <param name="player">The player to deliver the loot to.</param>
        void DeliverLoot(IPlayer player);

        /// <summary>
        /// Delivers the loot to the specified identity when the player is offline.
        /// </summary>
        /// <param name="identity">The identity to deliver the loot to.</param>
        void DeliverLootOffline(Identity identity);
    }
}
