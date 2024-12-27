using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Housing;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Housing
{
    public interface IResidence : IDatabaseCharacter, INetworkBuildable<ServerHousingProperties.Residence>
    {
        ulong Id { get; }
        ResidenceType Type { get; }
        ulong? OwnerId { get; }
        ulong? GuildOwnerId { get; set; }
        PropertyInfoId PropertyInfoId { get; set; }
        string Name { get; set; }
        ResidencePrivacyLevel PrivacyLevel { get; set; }
        ushort Wallpaper { get; set; }
        ushort Roof { get; set; }
        ushort Entryway { get; set; }
        ushort Door { get; set; }
        ushort Music { get; set; }
        ushort Ground { get; set; }
        ushort Sky { get; set; }
        ResidenceFlags Flags { get; set; }
        byte ResourceSharing { get; set; }
        byte GardenSharing { get; set; }

        bool IsCommunityResidence { get; }

        // <summary>
        /// <see cref="IResidenceMapInstance"/> this <see cref="IResidence"/> resides on.
        /// </summary>
        /// <remarks>
        /// This can either be an individual or shared residencial map.
        /// </remarks>
        IResidenceMapInstance Map { get; set; }

        /// <summary>
        /// Parent <see cref="IResidence"/> for this <see cref="IResidence"/>.
        /// </summary>
        /// <remarks>
        /// This will be set if the <see cref="IResidence"/> is part of a <see cref="ICommunity"/>.
        /// </remarks>
        IResidence Parent { get; set; }

        /// <summary>
        /// Return all <see cref="IResidenceChild"/>'s.
        /// </summary>
        /// <remarks>
        /// Only community residences will have child residences.
        /// </remarks>
        IEnumerable<IResidenceChild> GetChildren();

        /// <summary>
        /// Return <see cref="IResidenceChild"/> with supplied property info id.
        /// </summary>
        /// <remarks>
        /// Only community residences will have child residences.
        /// </remarks>
        IResidenceChild GetChild(PropertyInfoId propertyInfoId);

        /// <summary>
        /// Return <see cref="IResidenceChild"/> with supplied character id.
        /// </summary>
        /// <remarks>
        /// Only community residences will have child residences.
        /// </remarks>
        IResidenceChild GetChild(ulong characterId);

        /// <summary>
        /// Add child <see cref="IResidence"/> to parent <see cref="IResidence"/>.
        /// </summary>
        /// <remarks>
        /// Child residences can only be added to a community.
        /// </remarks>
        void AddChild(IResidence residence, bool temporary);

        /// <summary>
        /// Remove child <see cref="IResidence"/> to parent <see cref="IResidence"/>.
        /// </summary>
        /// <remarks>
        /// Child residences can only be removed from a community.
        /// </remarks>
        void RemoveChild(IResidence residence);

        /// <summary>
        /// Returns true if <see cref="IPlayer"/> can modify the <see cref="IResidence"/>.
        /// </summary>
        /// <remarks>
        /// This is valid for both community and individual residences.
        /// </remarks>
        bool CanModifyResidence(IPlayer player);

        /// <summary>
        /// Return all <see cref="IPlot"/>'s for the <see cref="IResidence"/>.
        /// </summary>
        IEnumerable<IPlot> GetPlots();

        /// <summary>
        /// Return all <see cref="IDecor"/> for the <see cref="IResidence"/>.
        /// </summary>
        IEnumerable<IDecor> GetDecor();

        /// <summary>
        /// Return all <see cref="IDecor"/> placed in the world for the <see cref="IResidence"/>.
        /// </summary>
        IEnumerable<IDecor> GetPlacedDecor();

        /// <summary>
        /// Return <see cref="IDecor"/> with the supplied id.
        /// </summary>
        IDecor GetDecor(ulong decorId);

        /// <summary>
        /// Create a new <see cref="IDecor"/> from supplied <see cref="HousingDecorInfoEntry"/> for <see cref="IResidence"/>.
        /// </summary>
        IDecor DecorCreate(HousingDecorInfoEntry entry);

        /// <summary>
        /// Create a new <see cref="IDecor"/> from an existing <see cref="IDecor"/>.
        /// </summary>
        /// <remarks>
        /// Copies all data from the source <see cref="IDecor"/> with a new id.
        /// </remarks>
        IDecor DecorCopy(IDecor decor);

        /// <summary>
        /// Remove existing <see cref="IDecor"/> from the <see cref="IResidence"/>.
        /// </summary>
        /// <remarks>
        /// This does not queue the <see cref="IDecor"/> for deletion from the database.
        /// This is intended to be used for <see cref="IDecor"/> that has yet to be saved to the database.
        /// </remarks>
        void DecorRemove(IDecor decor);

        /// <summary>
        /// Return <see cref="IPlot"/> at the supplied index.
        /// </summary>
        IPlot GetPlot(byte plotIndex);

        /// <summary>
        /// Return <see cref="IPlot"/> that matches the supploed Plot Info ID.
        /// </summary>
        IPlot GetPlot(uint plotInfoId);
    }
}