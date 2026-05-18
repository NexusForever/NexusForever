using System.Collections.Immutable;
using System.Numerics;
using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Matching;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Matching
{
    public class MatchingDataManager : Singleton<MatchingDataManager>, IMatchingDataManager
    {
        public bool DebugInstantQueue { get; set; }

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;
        private readonly IDatabaseManager databaseManager;

        public MatchingDataManager(
            IGameTableManager gameTableManager,
            IDatabaseManager databaseManager)
        {
            this.gameTableManager = gameTableManager;
            this.databaseManager  = databaseManager;
        }

        #endregion

        private ImmutableDictionary<uint, IMatchingMap> matchingMap;
        private ImmutableDictionary<uint, ImmutableList<IMatchingMap>> matchingMapXGameType;
        private ImmutableDictionary<Static.Matching.MatchType, ImmutableList<IMatchingMap>> matchingMapXMatchType;

        private ImmutableDictionary<(uint, byte), MapEntrance> mapEntrance;

        // the client seems to have hardcoded structures for each match type with a size of 16 bytes
        // offset 12 is some flags which we are storing here
        // needs more research, see MatchingGameLib.TransferIntoMatch for example of data
        private readonly ImmutableDictionary<Static.Matching.MatchType, uint> matchTypeFlags 
            = new Dictionary<Static.Matching.MatchType, uint>
            {
                { Static.Matching.MatchType.BattleGround,               0x0030 },
                { Static.Matching.MatchType.Dungeon,                    0x000B },
                { Static.Matching.MatchType.Adventure,                  0x000B },
                { Static.Matching.MatchType.Arena,                      0x000C },
                { Static.Matching.MatchType.Warplot,                    0x0074 },
                { Static.Matching.MatchType.RatedBattleground,          0x0030 },
                { Static.Matching.MatchType.OpenArena,                  0x0028 },
                { Static.Matching.MatchType.Event,                      0x0030 },
                { Static.Matching.MatchType.Shiphand,                   0x0101 },
                { Static.Matching.MatchType.WorldStory,                 0x0001 },
                { Static.Matching.MatchType.PrimeLevelDungeon,          0x0521 },
                { Static.Matching.MatchType.PrimeLevelExpedition,       0x0001 },
                { Static.Matching.MatchType.PrimeLevelAdventure,        0x0521 },
                { Static.Matching.MatchType.ScaledPrimeLevelAdventure,  0x000B },
                { Static.Matching.MatchType.ScaledPrimeLevelDungeon,    0x010B },
                { Static.Matching.MatchType.ScaledPrimeLevelExpedition, 0x0101 }
            }.ToImmutableDictionary();

        /// <summary>
        /// Initialise <see cref="IMatchingDataManager"/> with data from the game tables.
        /// </summary>
        public void Initialise()
        {
            InitialiseMapEntrances();

            var matchingMapBuilder          = ImmutableDictionary.CreateBuilder<uint, IMatchingMap>();
            var matchingMapXGameTypeBuilder = new Dictionary<uint, List<IMatchingMap>>();
            var matchingMapXTypeBuilder     = new Dictionary<Static.Matching.MatchType, List<IMatchingMap>>();

            foreach (MatchingGameMapEntry mapEntry in gameTableManager.MatchingGameMap.Entries)
            {
                MatchingGameTypeEntry typeEntry = gameTableManager.MatchingGameType.GetEntry(mapEntry.MatchingGameTypeId);
                if (typeEntry == null)
                    continue;

                var matchingMap = new MatchingMap()
                {
                    GameMapEntry  = mapEntry,
                    GameTypeEntry = typeEntry
                };

                matchingMapBuilder.Add(mapEntry.Id, matchingMap);

                if (!matchingMapXGameTypeBuilder.TryGetValue(mapEntry.MatchingGameTypeId, out List<IMatchingMap> gameTypeMatchingMaps))
                {
                    gameTypeMatchingMaps = [];
                    matchingMapXGameTypeBuilder.Add(mapEntry.MatchingGameTypeId, gameTypeMatchingMaps);
                }

                matchingMapXGameTypeBuilder[mapEntry.MatchingGameTypeId].Add(matchingMap);

                if (!matchingMapXTypeBuilder.TryGetValue(typeEntry.MatchTypeEnum, out List<IMatchingMap> matchTypematchingMaps))
                {
                    matchTypematchingMaps = [];
                    matchingMapXTypeBuilder.Add(typeEntry.MatchTypeEnum, matchTypematchingMaps);
                }

                matchingMapXTypeBuilder[typeEntry.MatchTypeEnum].Add(matchingMap);
            }

            matchingMap           = matchingMapBuilder.ToImmutable();
            matchingMapXGameType  = matchingMapXGameTypeBuilder.ToImmutableDictionary(t => t.Key, t => t.Value.ToImmutableList());
            matchingMapXMatchType = matchingMapXTypeBuilder.ToImmutableDictionary(t => t.Key, t => t.Value.ToImmutableList());
        }

        private void InitialiseMapEntrances()
        {
            var builder = ImmutableDictionary.CreateBuilder<(uint, byte), MapEntrance>();

            foreach (MapEntranceModel mapEntranceModel in databaseManager.GetDatabase<WorldDatabase>().GetMapEntrances())
            {
                WorldLocation2Entry entry = gameTableManager.WorldLocation2.GetEntry(mapEntranceModel.WorldLocationId);
                if (entry == null)
                    continue;

                var quaternion = new Quaternion(entry.Facing0, entry.Facing1, entry.Facing2, entry.Facing3);

                builder.Add((mapEntranceModel.MapId, mapEntranceModel.Team), new MapEntrance()
                {
                    MapId    = mapEntranceModel.MapId,
                    Team     = mapEntranceModel.Team,
                    Position = new Vector3(entry.Position0, entry.Position1, entry.Position2),
                    Rotation = quaternion.ToEuler(),
                });
            }

            mapEntrance = builder.ToImmutable();
        }

        /// <summary>
        /// Return <see cref="IMatchingMap"/> with supplied id.
        /// </summary>
        public IMatchingMap GetMatchingMap(uint matchingMapId)
        {
            return matchingMap.TryGetValue(matchingMapId, out IMatchingMap map) ? map : null;
        }

        /// <summary>
        /// Return <see cref="IMatchingMap"/> with supplied matching game type id.
        /// </summary>
        public IEnumerable<IMatchingMap> GetMatchingMaps(uint matchingGameTypeId)
        {
            return matchingMapXGameType.TryGetValue(matchingGameTypeId, out ImmutableList<IMatchingMap> maps) ? maps : Enumerable.Empty<IMatchingMap>();
        }

        /// <summary>
        /// Return <see cref="IMatchingMap"/> with supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        public IEnumerable<IMatchingMap> GetMatchingMaps(Static.Matching.MatchType matchType)
        {
            return matchingMapXMatchType.TryGetValue(matchType, out ImmutableList<IMatchingMap> maps) ? maps : Enumerable.Empty<IMatchingMap>();
        }

        /// <summary>
        /// Return if <see cref="Static.Matching.MatchType"/> is a PvP match type.
        /// </summary>
        public bool IsPvPMatchType(Static.Matching.MatchType matchType)
        {
            return matchType
                is Static.Matching.MatchType.BattleGround
                or Static.Matching.MatchType.Arena
                or Static.Matching.MatchType.Warplot
                or Static.Matching.MatchType.RatedBattleground
                or Static.Matching.MatchType.OpenArena;
        }

        /// <summary>
        /// Return if <see cref="Static.Matching.MatchType"/> enforces a specific team composition.
        /// </summary>
        public bool IsCompositionEnforced(Static.Matching.MatchType matchType)
        {
            return matchType
                is Static.Matching.MatchType.Dungeon
                or Static.Matching.MatchType.Adventure;
        }

        /// <summary>
        /// Return if <see cref="Static.Matching.MatchType"/> enforces a single faction queue.
        /// </summary>
        public bool IsSingleFactionEnforced(Static.Matching.MatchType matchType)
        {
            return matchType
                is Static.Matching.MatchType.Adventure
                or Static.Matching.MatchType.OpenArena;
        }

        /// <summary>
        /// Return default <see cref="Role"/>s for the supplied <see cref="Class"/>.
        /// </summary>
        public Role GetDefaultRole(Class @class)
        {
            // this is hard coded in the client LUA, see MatchMakingLib.GetEligibleRoles
            switch (@class)
            {
                case Class.Warrior:
                case Class.Engineer:
                case Class.Stalker:
                    return Role.Tank | Role.DPS;
                case Class.Esper:
                case Class.Medic:
                case Class.Spellslinger:
                    return Role.Healer | Role.DPS;
                default:
                    return Role.None;
            }
        }

        /// <summary>
        /// Return <see cref="IMapEntrance"/> for the supplied world and team.
        /// </summary>
        /// <remarks>
        /// Teams can have different entrances to the same map, for example in PvP.
        /// </remarks>
        public IMapEntrance GetMapEntrance(uint worldId, byte team)
        {
            return mapEntrance.TryGetValue((worldId, team), out MapEntrance entrance) ? entrance : null;
        }

        /// <summary>
        /// Return if a player can re-enter a match for the supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        /// <remarks>
        /// The client uses this to determine if the "Teleport to Instance" button should be enabled.
        /// </remarks>
        public bool CanReEnterMatch(Static.Matching.MatchType matchType)
        {
            return (matchTypeFlags[matchType] & 0x01) != 0;
        }
    }
}
