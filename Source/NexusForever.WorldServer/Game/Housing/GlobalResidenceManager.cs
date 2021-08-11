using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.Database;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Housing.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Housing
{
    public sealed class GlobalResidenceManager : Singleton<GlobalResidenceManager>, IUpdate
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        /// <summary>
        /// Id to be assigned to the next created residence.
        /// </summary>
        public ulong NextResidenceId => nextResidenceId++;
        private ulong nextResidenceId;

        /// <summary>
        /// Id to be assigned to the next created residence.
        /// </summary>
        public ulong NextDecorId => nextDecorId++;
        private ulong nextDecorId;

        private readonly Dictionary<ulong, Residence> residences = new();
        private readonly Dictionary<ulong, ulong> residenceOwnerCache = new();
        private readonly Dictionary<ulong, ulong> communityOwnerCache = new();
        private readonly Dictionary<string, ulong> residenceSearchCache = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, ulong> communitySearchCache = new(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<ulong, PublicResidence> visitableResidences = new();
        private readonly Dictionary<ulong, PublicCommunity> visitableCommunities = new();

        private double timeToSave = SaveDuration;

        private GlobalResidenceManager()
        {
        }

        public void Initialise()
        {
            nextResidenceId = DatabaseManager.Instance.CharacterDatabase.GetNextResidenceId() + 1ul;
            nextDecorId     = DatabaseManager.Instance.CharacterDatabase.GetNextDecorId() + 1ul;

            InitialiseResidences();
        }

        private void InitialiseResidences()
        {
            foreach (ResidenceModel model in DatabaseManager.Instance.CharacterDatabase.GetResidences())
            {
                if (model.OwnerId.HasValue)
                {
                    ICharacter character = CharacterManager.Instance.GetCharacterInfo(model.OwnerId.Value);
                    if (character == null)
                        throw new DatabaseDataException($"Character owner {model.OwnerId.Value} of residence {model.Id} is invalid!");

                    var residence = new Residence(model);
                    StoreResidence(residence, character);
                }
                else if (model.GuildOwnerId.HasValue)
                {
                    Community community = GlobalGuildManager.Instance.GetGuild<Community>(model.GuildOwnerId.Value);
                    if (community == null)
                        throw new DatabaseDataException($"Community owner {model.OwnerId.Value} of residence {model.Id} is invalid!");

                    var residence = new Residence(model);
                    community.Residence = residence;

                    StoreCommunity(residence, community);
                }
            }

            // create links between parents and children
            // only residences with both a character and guild owner are children
            foreach (Residence residence in residences.Values
                .Where(r => r.OwnerId.HasValue && r.GuildOwnerId.HasValue))
            {
                Community community = GlobalGuildManager.Instance.GetGuild<Community>(residence.GuildOwnerId.Value);
                if (community == null)
                    continue;

                GuildMember member = community.GetMember(residence.OwnerId.Value);
                if (member == null)
                    throw new DatabaseDataException($"Residence {residence.Id} is a child of {community.Residence.Id} but character {residence.OwnerId.Value} but isn't a member of community {community.Id}!");

                // temporary child status comes from the member data
                int communityPlotReservation = member?.CommunityPlotReservation ?? -1; 
                community.Residence.AddChild(residence, communityPlotReservation == -1);
            }

            log.Info($"Loaded {residences.Count} housing residences!");
        }

        public void Update(double lastTick)
        {
            timeToSave -= lastTick;
            if (timeToSave <= 0d)
            {
                var tasks = new List<Task>();
                foreach (Residence residence in residences.Values)
                    tasks.Add(DatabaseManager.Instance.CharacterDatabase.Save(residence.Save));

                Task.WaitAll(tasks.ToArray());

                timeToSave = SaveDuration;
            }
        }

        /// <summary>
        /// Create new <see cref="Residence"/> for supplied <see cref="Player"/>.
        /// </summary>
        public Residence CreateResidence(Player player)
        {
            var residence = new Residence(player);
            StoreResidence(residence, player);

            log.Trace($"Created new residence {residence.Id} for player {player.Name}.");
            return residence;
        }

        private void StoreResidence(Residence residence, ICharacter character)
        {
            residences.Add(residence.Id, residence);

            residenceOwnerCache.Add(residence.OwnerId.Value, residence.Id);
            residenceSearchCache.Add(character.Name, residence.Id);

            if (residence.PrivacyLevel == ResidencePrivacyLevel.Public)
                RegisterResidenceVists(residence, character);
        }

        /// <summary>
        /// Create new <see cref="Residence"/> for supplied <see cref="Community"/>.
        /// </summary>
        public Residence CreateCommunity(Community community)
        {
            var residence = new Residence(community);
            StoreCommunity(residence, community);

            log.Trace($"Created new residence {residence.Id} for community {community.Name}.");
            return residence;
        }

        private void StoreCommunity(Residence residence, Community community)
        {
            residences.Add(residence.Id, residence);

            communityOwnerCache.Add(residence.GuildOwnerId.Value, residence.Id);
            communitySearchCache.Add(community.Name, residence.Id);

            // community residences store the privacy level in the community it self as a guild flag
            if ((community.Flags & GuildFlag.CommunityPrivate) == 0)
            {
                ICharacter character = CharacterManager.Instance.GetCharacterInfo(community.LeaderId.Value);
                RegisterCommunityVisits(residence, community, character);
            }
        }

        /// <summary>
        /// Return existing <see cref="Residence"/> by supplied residence id.
        /// </summary>
        public Residence GetResidence(ulong residenceId)
        {
            return residences.TryGetValue(residenceId, out Residence residence) ? residence : null;
        }

        /// <summary>
        /// Return existing <see cref="Residence"/> by supplied owner name.
        /// </summary>
        public Residence GetResidenceByOwner(string name)
        {
            return residenceSearchCache.TryGetValue(name, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// return existing <see cref="Residence"/> by supplied owner id.
        /// </summary>
        public Residence GetResidenceByOwner(ulong characterId)
        {
            return residenceOwnerCache.TryGetValue(characterId, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// Return existing <see cref="Residence"/> by supplied community name.
        /// </summary>
        public Residence GetCommunityByOwner(string name)
        {
            return communitySearchCache.TryGetValue(name, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// return existing <see cref="Residence"/> by supplied owner id.
        /// </summary>
        public Residence GetCommunityByOwner(ulong communityId)
        {
            return communityOwnerCache.TryGetValue(communityId, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// Remove an existing <see cref="Residence"/> by supplied character name.
        /// </summary>
        public void RemoveResidence(string name)
        {
            if (!residenceSearchCache.TryGetValue(name, out ulong residenceId))
                return;

            if (!residences.TryGetValue(residenceId, out Residence residence))
                return;

            if (residence.Parent != null)
            {
                if (residence.Map != null)
                    residence.Map.RemoveChild(residence);
                else
                    residence.Parent.RemoveChild(residence);
            }
            else
                residence.Map?.Unload();

            if (residence.PrivacyLevel == ResidencePrivacyLevel.Public)
                DeregisterResidenceVists(residence.Id);

            residences.Remove(residence.Id);
            residenceOwnerCache.Remove(residence.OwnerId.Value);
            residenceSearchCache.Remove(name);
        }

        public ResidenceEntrance GetResidenceEntrance(PropertyInfoId propertyInfoId)
        {
            HousingPropertyInfoEntry propertyEntry = GameTableManager.Instance.HousingPropertyInfo.GetEntry((ulong)propertyInfoId);
            if (propertyEntry == null)
                throw new HousingException();

            WorldLocation2Entry locationEntry = GameTableManager.Instance.WorldLocation2.GetEntry(propertyEntry.WorldLocation2Id);
            return new ResidenceEntrance(locationEntry);
        }

        /// <summary>
        /// Register residence as visitable, this allows anyone to visit through the random property feature.
        /// </summary>
        public void RegisterResidenceVists(Residence residence, ICharacter character)
        {
            visitableResidences.Add(residence.Id, new PublicResidence
            {
                ResidenceId = residence.Id,
                Owner       = character.Name,
                Name        = residence.Name
            });
        }

        /// <summary>
        /// Register community as visitable, this allows anyone to visit through the random property feature.
        /// </summary>
        public void RegisterCommunityVisits(Residence residence, Community community, ICharacter character)
        {
            visitableCommunities.Add(residence.Id, new PublicCommunity
            {
                NeighbourhoodId = community.Id,
                Owner           = character.Name,
                Name            = community.Name
            });
        }

        /// <summary>
        /// Deregister residence as visitable, this prevents anyone from visiting through the random property feature.
        /// </summary>
        public void DeregisterResidenceVists(ulong residenceId)
        {
            visitableResidences.Remove(residenceId);
        }

        /// <summary>
        /// Deregister community as visitable, this prevents anyone from visiting through the random property feature.
        /// </summary>
        public void DeregisterCommunityVists(ulong residenceId)
        {
            visitableCommunities.Remove(residenceId);
        }

        /// <summary>
        /// Return 50 random registered visitable residences.
        /// </summary>
        public IEnumerable<PublicResidence> GetRandomVisitableResidences()
        {
            // unsure if this is how it was done on retail, might need to be tweaked
            var random = new Random();
            return visitableResidences
                .Values
                .OrderBy(r => random.Next())
                .Take(50);
        }

        /// <summary>
        /// Return 50 random registered visitable communities.
        /// </summary>
        public IEnumerable<PublicCommunity> GetRandomVisitableCommunities()
        {
            // unsure if this is how it was done on retail, might need to be tweaked
            var random = new Random();
            return visitableCommunities
                .Values
                .OrderBy(r => random.Next())
                .Take(50);
        }
    }
}
