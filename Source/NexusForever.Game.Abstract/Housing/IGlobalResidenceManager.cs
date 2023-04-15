using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Housing
{
    public interface IGlobalResidenceManager : IUpdate
    {
        /// <summary>
        /// Id to be assigned to the next created residence.
        /// </summary>
        ulong NextResidenceId { get; }

        /// <summary>
        /// Id to be assigned to the next created residence.
        /// </summary>
        ulong NextDecorId { get; }

        /// <summary>
        /// Initialise <see cref="IGlobalResidenceManager"/> and any related resources.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Shutdown <see cref="IGlobalResidenceManager"/> and any related resources.
        /// </summary>
        /// <remarks>
        /// This will force save all residences.
        /// </remarks>
        void Shutdown();

        /// <summary>
        /// Create new <see cref="IResidence"/> for supplied <see cref="IPlayer"/>.
        /// </summary>
        IResidence CreateResidence(IPlayer player);

        /// <summary>
        /// Create new <see cref="IResidence"/> for supplied <see cref="ICommunity"/>.
        /// </summary>
        IResidence CreateCommunity(ICommunity community);

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied residence id.
        /// </summary>
        IResidence GetResidence(ulong residenceId);

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied owner name.
        /// </summary>
        IResidence GetResidenceByOwner(string name);

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied owner id.
        /// </summary>
        IResidence GetResidenceByOwner(ulong characterId);

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied community name.
        /// </summary>
        IResidence GetCommunityByOwner(string name);

        /// <summary>
        /// return existing <see cref="IResidence"/> by supplied owner id.
        /// </summary>
        IResidence GetCommunityByOwner(ulong communityId);

        /// <summary>
        /// Remove an existing <see cref="IResidence"/> by supplied character name.
        /// </summary>
        void RemoveResidence(string name);

        IResidenceEntrance GetResidenceEntrance(PropertyInfoId propertyInfoId);

        /// <summary>
        /// Register residence as visitable, this allows anyone to visit through the random property feature.
        /// </summary>
        void RegisterResidenceVists(IResidence residence, string name);

        /// <summary>
        /// Register community as visitable, this allows anyone to visit through the random property feature.
        /// </summary>
        void RegisterCommunityVisits(IResidence residence, ICommunity community, string name);

        /// <summary>
        /// Deregister residence as visitable, this prevents anyone from visiting through the random property feature.
        /// </summary>
        void DeregisterResidenceVists(ulong residenceId);

        /// <summary>
        /// Deregister community as visitable, this prevents anyone from visiting through the random property feature.
        /// </summary>
        void DeregisterCommunityVists(ulong residenceId);

        /// <summary>
        /// Return 50 random registered visitable residences.
        /// </summary>
        IEnumerable<IPublicResidence> GetRandomVisitableResidences();

        /// <summary>
        /// Return 50 random registered visitable communities.
        /// </summary>
        IEnumerable<IPublicCommunity> GetRandomVisitableCommunities();
    }
}