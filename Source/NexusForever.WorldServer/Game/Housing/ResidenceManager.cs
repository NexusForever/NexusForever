using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Game.Entity;
using ResidenceModel = NexusForever.WorldServer.Database.Character.Model.Residence;

namespace NexusForever.WorldServer.Game.Housing
{
    public sealed class ResidenceManager : Singleton<ResidenceManager>, IUpdate
    {
        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        /// <summary>
        /// Id to be assigned to the next created residence.
        /// </summary>
        public ulong NextResidenceId => nextResidenceId++;

        /// <summary>
        /// Id to be assigned to the next created residence.
        /// </summary>
        public ulong NextDecorId => nextDecorId++;

        private ulong nextResidenceId;
        private ulong nextDecorId;

        private static readonly ConcurrentDictionary</*residenceId*/ ulong, Residence> residences = new ConcurrentDictionary<ulong, Residence>();
        private ConcurrentDictionary</*owner*/ string, ulong /*residenceId*/> ownerCache = new ConcurrentDictionary<string, ulong>();

        private readonly Dictionary<ulong, PublicResidence> visitableResidences = new Dictionary<ulong, PublicResidence>();

        private double timeToSave = SaveDuration;

        private ResidenceManager()
        {
        }

        public void Initialise()
        {
            nextResidenceId = CharacterDatabase.GetNextResidenceId() + 1ul;
            nextDecorId     = CharacterDatabase.GetNextDecorId() + 1ul;

            foreach (ResidenceModel residence in CharacterDatabase.GetPublicResidences())
                RegisterResidenceVists(residence.Id, residence.Owner.Name, residence.Name);
        }

        public void Update(double lastTick)
        {
            timeToSave -= lastTick;
            if (timeToSave <= 0d)
            {
                var tasks = new List<Task>();
                foreach (Residence residence in residences.Values)
                    tasks.Add(CharacterDatabase.SaveResidence(residence));

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
            residences.TryAdd(residence.Id, residence);
            ownerCache.TryAdd(player.Name, residence.Id);
            return residence;
        }

        /// <summary>
        /// Return existing <see cref="Residence"/> by supplied residence id, if not locally cached it will be retrieved from the database.
        /// </summary>
        public async Task<Residence> GetResidence(ulong residenceId)
        {
            Residence residence = GetCachedResidence(residenceId);
            if (residence != null)
                return residence;

            ResidenceModel model = await CharacterDatabase.GetResidence(residenceId);
            if (model == null)
                return null;

            residence = new Residence(model);
            residences.TryAdd(residence.Id, residence);
            ownerCache.TryAdd(model.Owner.Name, residence.Id);
            return residence;
        }

        /// <summary>
        /// Return existing <see cref="Residence"/> by supplied owner name, if not locally cached it will be retrieved from the database.
        /// </summary>
        public async Task<Residence> GetResidence(string name)
        {
            if (ownerCache.TryGetValue(name, out ulong residenceId))
                return GetCachedResidence(residenceId);

            ResidenceModel model = await CharacterDatabase.GetResidence(name);
            if (model == null)
                return null;

            var residence = new Residence(model);
            residences.TryAdd(residence.Id, residence);
            ownerCache.TryAdd(name, residence.Id);
            return residence;
        }

        /// <summary>
        /// Return cached <see cref="Residence"/> by supplied residence id.
        /// </summary>
        public Residence GetCachedResidence(ulong residenceId)
        {
            return residences.TryGetValue(residenceId, out Residence residence) ? residence : null;
        }

        public ResidenceEntrance GetResidenceEntrance(Residence residence)
        {
            HousingPropertyInfoEntry propertyEntry = GameTableManager.Instance.HousingPropertyInfo.GetEntry(residence.PropertyInfoId);
            if (propertyEntry == null)
                throw new HousingException();

            WorldLocation2Entry locationEntry = GameTableManager.Instance.WorldLocation2.GetEntry(propertyEntry.WorldLocation2Id);
            return new ResidenceEntrance(locationEntry);
        }

        /// <summary>
        /// Register residence as visitable, this allows anyone to visit through the random property feature.
        /// </summary>
        public void RegisterResidenceVists(ulong residenceId, string owner, string name)
        {
            visitableResidences.Add(residenceId, new PublicResidence(residenceId, owner, name));
        }

        /// <summary>
        /// Deregister residence as visitable, this prevents anyone from visiting through the random property feature.
        /// </summary>
        public void DeregisterResidenceVists(ulong residenceId)
        {
            visitableResidences.Remove(residenceId);
        }

        /// <summary>
        /// Return 50 random registered visitable residences.
        /// </summary>
        public IEnumerable<PublicResidence> GetRandomVisitableResidences()
        {
            var random = new Random();
            return visitableResidences
                .Values
                .OrderBy(r => random.Next())
                .Take(50);
        }
    }
}
