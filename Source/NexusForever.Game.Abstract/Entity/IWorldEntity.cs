using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Chat;
using NexusForever.Game.Abstract.Entity.Creature;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Command;
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
        public WorldZoneEntry Zone { get; }

        uint EntityId { get; }

        uint CreatureId { get; }
        ICreatureInfo CreatureInfo { get; set; }
        uint DisplayInfoId { get; }
        Creature2DisplayInfoEntry CreatureDisplayEntry { get; set; }
        ushort OutfitInfoId { get; }
        Creature2OutfitInfoEntry CreatureOutfitEntry { get; set; }

        Faction Faction1 { get; set; } // current primary faction
        Faction Faction2 { get; set; } // original faction

        byte QuestChecklistIdx { get; }

        ushort WorldSocketId { get; }
        ulong ActivePropId { get; }

        EntitySplineModel Spline { get; }

        Vector3 LeashPosition { get; }
        float LeashRange { get; }
        IMovementManager MovementManager { get; }

        uint Health { get; }
        uint MaxHealth { get; set; }
        uint Shield { get; set; }
        uint MaxShieldCapacity { get; set; }

        float Endurance { get; set; }
        float Focus { get; set; }
        float Dash { get; set; }
        float Resource1 { get; set; }
        float Resource3 { get; set; }
        float Resource4 { get; set; }
        float InterruptArmour { get; set; }
        int MaxInterruptArmour { get; set; }

        uint Level { get; set; }
        bool Sheathed { get; set; }

        StandState StandState { get; set; }

        /// <summary>
        /// Collection of guids currently targeting this <see cref="IWorldEntity"/>.
        /// </summary>
        IEnumerable<uint> TargetingGuids { get; }

        /// <summary>
        /// Guid of the <see cref="IPlayer"/> currently controlling this <see cref="IWorldEntity"/>.
        /// </summary>
        uint? ControllerGuid { get; set; }

        /// <summary>
        /// Guid of the <see cref="IWorldEntity"/> the <see cref="IWorldEntity"/> is a passenger on.
        /// </summary>
        uint? PlatformGuid { get; }

        /// <summary>
        /// Guid of the <see cref="IWorldEntity"/> that summoned this <see cref="IWorldEntity"/>.
        /// </summary>
        uint? SummonerGuid { get; set; }

        /// <summary>
        /// An entity factory to summon child entities.
        /// </summary>
        /// <remarks>
        /// Any entities summoned by this <see cref="IWorldEntity"/> will be removed when this <see cref="IWorldEntity"/> is removed.
        /// </remarks>
        IEntitySummonFactory SummonFactory { get; }

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> with supplied <see cref="ICreatureInfo"/>.
        /// </summary>
        void Initialise(ICreatureInfo creatureInfo);

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> from an existing database model.
        /// </summary>
        void Initialise(ICreatureInfo creatureInfo, EntityModel model);

        ServerEntityCreate BuildCreatePacket(bool initialCommands);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is activated.
        /// </summary>
        void OnActivate(IPlayer activator);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is cast activated.
        /// </summary>
        void OnActivateCast(IPlayer activator, uint interactionId);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/>'s activate succeeds.
        /// </summary>
        void OnActivateSuccess(IPlayer activator);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/>'s activation fails.
        /// </summary>
        void OnActivateFail(IPlayer activator);

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
        /// Calculate the primary value for <see cref="Property"/>.
        /// </summary>
        void CalculateProperty(Property property);

        /// <summary>
        /// Set <see cref="IWorldEntity"/> to broadcast <see cref="Property"/> on next world update.
        /// </summary>
        void SetPropertyEmit(Property property);

        /// <summary>
        /// Return the <see cref="uint"/> value of the supplied <see cref="Stat"/> as an <see cref="Enum"/>.
        /// </summary>
        T? GetStatEnum<T>(Static.Entity.Stat stat) where T : struct, Enum;

        /// <summary>
        /// Get the current value of the <see cref="Stat"/> mapped to <see cref="Vital"/>.
        /// </summary>
        float GetVitalValue(Vital vital);

        /// <summary>
        /// Set the stat value for the provided <see cref="Vital"/>.
        /// </summary>
        void SetVital(Vital vital, float value);

        /// <summary>
        /// Modify the current stat value for the <see cref="Vital"/>.
        /// </summary>
        void ModifyVital(Vital vital, float value);

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all visible <see cref="IPlayer"/>'s in range.
        /// </summary>
        void EnqueueToVisible(IWritable message, bool includeSelf = false);

        /// <summary>
        /// Set primary faction to supplied <see cref="Faction"/>.
        /// </summary>
        void SetFaction(Faction factionId);

        /// <summary>
        /// Set temporary faction to supplied <see cref="Faction"/>.
        /// </summary>
        void SetTemporaryFaction(Faction factionId);

        /// <summary>
        /// Remove temporary faction and revert to primary faction.
        /// </summary>
        void RemoveTemporaryFaction();

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
        void Talk(IChatMessageBuilder builder, float range, IPlayer exclude = null);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is targeted by another <see cref="IUnitEntity"/>.
        /// </summary>
        /// <remarks>
        /// While any entity can be targeted, only <see cref="IUnitEntity"/> can target.
        /// </remarks>
        void OnTargeted(IUnitEntity source);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is untargeted by another <see cref="IUnitEntity"/>.
        /// </summary>
        void OnUntargeted(IUnitEntity source);

        /// <summary>
        /// Set platform to suppled <see cref="IWorldEntity"/> with optional position and rotation offsets.
        /// </summary>
        void SetPlatform(IWorldEntity entity, Vector3 position = default, Vector3 rotation = default);

        /// <summary>
        /// Add <see cref="IWorldEntity"/> as a passenger on this <see cref="IWorldEntity"/>.
        /// </summary>
        void AddPlatformPassenger(IWorldEntity passenger);

        /// <summary>
        /// Remove <see cref="IWorldEntity"/> as a passenger on this <see cref="IWorldEntity"/>.
        /// </summary>
        void RemovePlatformPassenger(IWorldEntity passenger);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> summons another <see cref="IWorldEntity"/>.
        /// </summary>
        void OnSummon(IWorldEntity entity);

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> unsummons another <see cref="IWorldEntity"/>.
        /// </summary>
        void OnUnsummon(IWorldEntity entity);

        /// <summary>
        /// Invoked when an <see cref="IEntityCommand"/> has finialised for <see cref="IWorldEntity"/>.
        /// </summary>
        void OnEntityCommandFinalise(IEntityCommand command);
    }
}
