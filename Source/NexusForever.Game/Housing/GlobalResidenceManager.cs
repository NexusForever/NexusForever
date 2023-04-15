using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Character;
using NexusForever.Game.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Housing
{
    public sealed class GlobalResidenceManager : Singleton<GlobalResidenceManager>, IGlobalResidenceManager
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

        private readonly Dictionary<ulong, IResidence> residences = new();
        private readonly Dictionary<ulong, ulong> residenceOwnerCache = new();
        private readonly Dictionary<ulong, ulong> communityOwnerCache = new();
        private readonly Dictionary<string, ulong> residenceSearchCache = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, ulong> communitySearchCache = new(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<ulong, IPublicResidence> visitableResidences = new();
        private readonly Dictionary<ulong, IPublicCommunity> visitableCommunities = new();

        private double timeToSave = SaveDuration;

        private GlobalResidenceManager()
        {
        }

        /// <summary>
        /// Initialise <see cref="IGlobalResidenceManager"/> and any related resources.
        /// </summary>
        public void Initialise()
        {
            nextResidenceId = DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetNextResidenceId() + 1ul;
            nextDecorId     = DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetNextDecorId() + 1ul;

            InitialiseResidences();
        }

        private void InitialiseResidences()
        {
            foreach (ResidenceModel model in DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetResidences())
            {
                if (model.OwnerId.HasValue)
                {
                    ICharacter character = CharacterManager.Instance.GetCharacter(model.OwnerId.Value);
                    if (character == null)
                        throw new DatabaseDataException($"Character owner {model.OwnerId.Value} of residence {model.Id} is invalid!");

                    var residence = new Residence(model);
                    StoreResidence(residence, character.Name);
                }
                else if (model.GuildOwnerId.HasValue)
                {
                    ICommunity community = GlobalGuildManager.Instance.GetGuild<ICommunity>(model.GuildOwnerId.Value);
                    if (community == null)
                        throw new DatabaseDataException($"Community owner {model.OwnerId.Value} of residence {model.Id} is invalid!");

                    var residence = new Residence(model);
                    community.Residence = residence;

                    StoreCommunity(residence, community);
                }
            }

            // create links between parents and children
            // only residences with both a character and guild owner are children
            foreach (IResidence residence in residences.Values
                .Where(r => r.OwnerId.HasValue && r.GuildOwnerId.HasValue))
            {
                ICommunity community = GlobalGuildManager.Instance.GetGuild<ICommunity>(residence.GuildOwnerId.Value);
                if (community == null)
                    continue;

                IGuildMember member = community.GetMember(residence.OwnerId.Value);
                if (member == null)
                    throw new DatabaseDataException($"Residence {residence.Id} is a child of {community.Residence.Id} but character {residence.OwnerId.Value} but isn't a member of community {community.Id}!");

                // temporary child status comes from the member data
                int communityPlotReservation = member?.CommunityPlotReservation ?? -1; 
                community.Residence.AddChild(residence, communityPlotReservation == -1);
            }

            log.Info($"Loaded {residences.Count} housing residences!");
        }

        /// <summary>
        /// Shutdown <see cref="IGlobalResidenceManager"/> and any related resources.
        /// </summary>
        /// <remarks>
        /// This will force save all residences.
        /// </remarks>
        public void Shutdown()
        {
            log.Info("Shutting down residence manager...");

            SaveResidences();
        }

        public void Update(double lastTick)
        {
            timeToSave -= lastTick;
            if (timeToSave <= 0d)
            {
                SaveResidences();
                timeToSave = SaveDuration;
            }
        }

        private void SaveResidences()
        {
            var tasks = new List<Task>();
            foreach (IResidence residence in residences.Values)
                tasks.Add(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(residence.Save));

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Create new <see cref="IResidence"/> for supplied <see cref="IPlayer"/>.
        /// </summary>
        public IResidence CreateResidence(IPlayer player)
        {
            var residence = new Residence(player);
            StoreResidence(residence, player.Name);

            log.Trace($"Created new residence {residence.Id} for player {player.Name}.");
            return residence;
        }

        private void StoreResidence(IResidence residence, string name)
        {
            residences.Add(residence.Id, residence);

            residenceOwnerCache.Add(residence.OwnerId.Value, residence.Id);
            residenceSearchCache.Add(name, residence.Id);

            if (residence.PrivacyLevel == ResidencePrivacyLevel.Public)
                RegisterResidenceVists(residence, name);
        }

        /// <summary>
        /// Create new <see cref="IResidence"/> for supplied <see cref="ICommunity"/>.
        /// </summary>
        public IResidence CreateCommunity(ICommunity community)
        {
            var residence = new Residence(community);
            StoreCommunity(residence, community);

            log.Trace($"Created new residence {residence.Id} for community {community.Name}.");
            return residence;
        }

        private void StoreCommunity(IResidence residence, ICommunity community)
        {
            residences.Add(residence.Id, residence);

            communityOwnerCache.Add(residence.GuildOwnerId.Value, residence.Id);
            communitySearchCache.Add(community.Name, residence.Id);

            // community residences store the privacy level in the community it self as a guild flag
            if ((community.Flags & GuildFlag.CommunityPrivate) == 0)
            {
                ICharacter character = CharacterManager.Instance.GetCharacter(community.LeaderId.Value);
                RegisterCommunityVisits(residence, community, character.Name);
            }
        }

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied residence id.
        /// </summary>
        public IResidence GetResidence(ulong residenceId)
        {
            return residences.TryGetValue(residenceId, out IResidence residence) ? residence : null;
        }

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied owner name.
        /// </summary>
        public IResidence GetResidenceByOwner(string name)
        {
            return residenceSearchCache.TryGetValue(name, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied owner id.
        /// </summary>
        public IResidence GetResidenceByOwner(ulong characterId)
        {
            return residenceOwnerCache.TryGetValue(characterId, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// Return existing <see cref="IResidence"/> by supplied community name.
        /// </summary>
        public IResidence GetCommunityByOwner(string name)
        {
            return communitySearchCache.TryGetValue(name, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// return existing <see cref="IResidence"/> by supplied owner id.
        /// </summary>
        public IResidence GetCommunityByOwner(ulong communityId)
        {
            return communityOwnerCache.TryGetValue(communityId, out ulong residenceId) ? GetResidence(residenceId) : null;
        }

        /// <summary>
        /// Remove an existing <see cref="IResidence"/> by supplied character name.
        /// </summary>
        public void RemoveResidence(string name)
        {
            if (!residenceSearchCache.TryGetValue(name, out ulong residenceId))
                return;

            if (!residences.TryGetValue(residenceId, out IResidence residence))
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

        public IResidenceEntrance GetResidenceEntrance(PropertyInfoId propertyInfoId)
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
        public void RegisterResidenceVists(IResidence residence, string name)
        {
            visitableResidences.Add(residence.Id, new PublicResidence
            {
                ResidenceId = residence.Id,
                Owner       = name,
                Name        = residence.Name
            });
        }

        /// <summary>
        /// Register community as visitable, this allows anyone to visit through the random property feature.
        /// </summary>
        public void RegisterCommunityVisits(IResidence residence, ICommunity community, string name)
        {
            visitableCommunities.Add(residence.Id, new PublicCommunity
            {
                NeighbourhoodId = community.Id,
                Owner           = name,
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
        public IEnumerable<IPublicResidence> GetRandomVisitableResidences()
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
        public IEnumerable<IPublicCommunity> GetRandomVisitableCommunities()
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
