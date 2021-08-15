using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Entity
{
    /// <summary>
    /// An <see cref="IWorldEntity"/> is an extension to <see cref="IGridEntity"/> which is also sent to the client and visible in the game world.
    /// </summary>
    public interface IWorldEntity : IGridEntity
    {
        EntityType Type { get; }
        EntityCreateFlag CreateFlags { get; set; }
        Vector3 Rotation { get; set; }

        uint EntityId { get; }
        uint CreatureId { get; set; }
        Creature2Entry CreatureEntry { get; }
        uint DisplayInfo { get; set; }
        Creature2DisplayInfoEntry CreatureDisplayEntry { get; }
        ushort OutfitInfo { get; set; }
        Creature2OutfitInfoEntry CreatureOutfitEntry { get; }
        Faction Faction1 { get; set; }
        Faction Faction2 { get; set; }

        ushort WorldSocketId { get; }
        ulong ActivePropId { get; }

        Vector3 LeashPosition { get; }
        float LeashRange { get; }
        IMovementManager MovementManager { get; }

        uint Health { get; }
        uint Shield { get; }
        uint Level { get; set; }
        bool Sheathed { get; set; }

        bool IsAlive { get; }

        /// <summary>
        /// Guid of the <see cref="IWorldEntity"/> currently targeted.
        /// </summary>
        uint TargetGuid { get; set; }

        /// <summary>
        /// Guid of the <see cref="IPlayer"/> currently controlling this <see cref="IWorldEntity"/>.
        /// </summary>
        uint ControllerGuid { get; set; }

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> from an existing database model.
        /// </summary>
        void Initialise(EntityModel model);

        ServerEntityCreate BuildCreatePacket();

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is activated.
        /// </summary>
        void OnActivate(IPlayer activator);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is cast activated.
        /// </summary>
        void OnActivateCast(IPlayer activator);

        /// <summary>
        /// Return a collection of <see cref="IItemVisual"/> for <see cref="IWorldEntity"/>.
        /// </summary>
        IEnumerable<IItemVisual> GetVisuals();

        /// <summary>
        /// Set <see cref="IWorldEntity"/> to broadcast all <see cref="IItemVisual"/> on next world update.
        /// </summary>
        void SetVisualEmit(bool status);

        /// <summary>
        /// Add or update <see cref="IItemVisual"/> at <see cref="ItemSlot"/> with supplied data.
        /// </summary>
        void AddVisual(ItemSlot slot, ushort displayId, ushort colourSetId = 0, int dyeData = 0);

        /// <summary>
        /// Add or update <see cref="IItemVisual"/>.
        /// </summary>
        void AddVisual(IItemVisual visual);

        /// <summary>
        /// Remove <see cref="IItemVisual"/> at supplied <see cref="ItemSlot"/>.
        /// </summary>
        void RemoveVisual(ItemSlot slot);

        /// <summary>
        /// Return a collection of <see cref="IPropertyValue"/> for <see cref="IWorldEntity"/>.
        /// </summary>
        IEnumerable<IPropertyValue> GetProperties();

        /// <summary>
        /// Get <see cref="IPropertyValue"/> for <see cref="IWorldEntity"/> <see cref="Property"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Property"/> doesn't exist it will be created with the default value specified in the GameTable.
        /// </remarks>
        IPropertyValue GetProperty(Property property);

        /// <summary>
        /// Returns the base value for <see cref="IWorldEntity"/> <see cref="Property"/>.
        /// </summary>
        float GetPropertyBaseValue(Property property);

        /// <summary>
        /// Returns the primary value for <see cref="IWorldEntity"/> <see cref="Property"/>.
        /// </summary>
        float GetPropertyValue(Property property);

        /// <summary>
        /// Sets the base value and calculate primary value for <see cref="Property"/>.
        /// </summary>
        void SetBaseProperty(Property property, float value);

        /// <summary>
        /// Set <see cref="IWorldEntity"/> to broadcast <see cref="Property"/> on next world update.
        /// </summary>
        void SetPropertyEmit(Property property);

        /// <summary>
        /// Return the <see cref="uint"/> value of the supplied <see cref="Stat"/> as an <see cref="Enum"/>.
        /// </summary>
        T? GetStatEnum<T>(Stat stat) where T : struct, Enum;

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all visible <see cref="IPlayer"/>'s in range.
        /// </summary>
        void EnqueueToVisible(IWritable message, bool includeSelf = false);

        /// <summary>
        /// Return <see cref="Disposition"/> between <see cref="IWorldEntity"/> and <see cref="Faction"/>.
        /// </summary>
        Disposition GetDispositionTo(Faction factionId, bool primary = true);

        /// <summary>
        /// Broadcast NPC say chat message to <see cref="IPlayer"/> in supplied range.
        /// </summary>
        void NpcSay(string text, float range = 155f);

        /// <summary>
        /// Broadcast NPC yell chat message to <see cref="IPlayer"/> in supplied range.
        /// </summary>
        void NpcYell(string text, float range = 155f);

        /// <summary>
        /// Broadcast chat message built from <see cref="IChatMessageBuilder"/> to <see cref="IPlayer"/> in supplied range.
        /// </summary>
        void Talk(IChatMessageBuilder builder, float range, IGridEntity exclude = null);

        /// <summary>
        /// Modify this Entity's Health by the given value (Negative for Damage, Positive for Healing).
        /// </summary>
        void ModifyHealth(long health);
    }
}