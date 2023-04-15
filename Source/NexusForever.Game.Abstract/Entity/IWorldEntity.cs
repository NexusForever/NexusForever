using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

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
        Dictionary<Property, IPropertyValue> Properties { get; }

        uint EntityId { get; }
        uint CreatureId { get; }
        uint DisplayInfo { get; }
        ushort OutfitInfo { get; }
        Faction Faction1 { get; set; }
        Faction Faction2 { get; set; }

        ushort WorldSocketId { get; }
        ulong ActivePropId { get; }

        Vector3 LeashPosition { get; }
        float LeashRange { get; }
        IMovementManager MovementManager { get; }

        uint Health { get; }
        float Shield { get; }
        uint Level { get; set; }
        bool Sheathed { get; set; }

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
        /// Return the <see cref="uint"/> value of the supplied <see cref="Stat"/> as an <see cref="Enum"/>.
        /// </summary>
        T? GetStatEnum<T>(Stat stat) where T : struct, Enum;

        /// <summary>
        /// Update <see cref="ItemVisual"/> for multiple supplied <see cref="ItemSlot"/>.
        /// </summary>
        void SetAppearance(IEnumerable<ItemVisual> visuals);

        /// <summary>
        /// Update <see cref="ItemVisual"/> for supplied <see cref="ItemVisual"/>.
        /// </summary>
        void SetAppearance(ItemVisual visual);

        IEnumerable<ItemVisual> GetAppearance();

        /// <summary>
        /// Update the display info for the <see cref="IWorldEntity"/>, this overrides any other appearance changes.
        /// </summary>
        void SetDisplayInfo(uint displayInfo);

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all visible <see cref="IPlayer"/>'s in range.
        /// </summary>
        void EnqueueToVisible(IWritable message, bool includeSelf = false);

        /// <summary>
        /// Return <see cref="Disposition"/> between <see cref="IWorldEntity"/> and <see cref="Faction"/>.
        /// </summary>
        Disposition GetDispositionTo(Faction factionId, bool primary = true);
    }
}